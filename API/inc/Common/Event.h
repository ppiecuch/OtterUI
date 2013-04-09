#pragma once
#include "Array.h"
#include "Types.h"

namespace Otter
{	
	struct Signature
	{
		uintptr_t mUpper;
		uintptr_t mLower;

		bool operator==(const Signature& rhs) const
		{
			return	(mUpper == rhs.mUpper) &&
					(mLower == rhs.mLower);
		}
	};

	/**
	 * Simple Functor Base class that takes 2 parameters
	 */
	template<class T = void*>
	class IFunctor
	{
	public:
		IFunctor() { }
		virtual ~IFunctor() { }

	public:
		virtual void	operator()(void*, T) = 0;

		const Signature& GetSignature() const { return mSignature; }

	protected:

		Signature	mSignature;
	};

	/**
	 * Implementation for a functor that takes two parameters
	 */
	template<class T, class P>
	class Functor : public IFunctor<P>
	{	
	public:

		typedef void (T::*Function_t)(void*, P);

	public:

		/**
		 * Constructor
		 */
		Functor()
		{
			mTarget = 0;
			mFunction = 0;
		}

		/**
		 * Constructor with an initial target instance and function signature
		 */
		Functor(T* pTarget, Function_t fp)
		{
			mTarget = pTarget;
			mFunction = fp;
			
			this->mSignature.mUpper = (uintptr_t)mTarget;
			this->mSignature.mLower = *(uintptr_t*)&mFunction;
		}

		/**
		 * Executes the functor
		 */
		virtual void operator()(void* pSender, P pContext)
		{
			if(mTarget && mFunction)
				(mTarget->*mFunction)(pSender, pContext);
		}
		
	protected:
		
		T*			mTarget;
		Function_t	mFunction;
	};
	
	/**
	 * The Event maintains a list of functors and calls each of them
	 * in order when raised. 
	 */
	template<class P>
	class Event
	{
	public:
		/**
		 * Adds an event handler.  Does not allow duplicates.
		 */
		template <class T>
		void AddHandler(T* pTarget, void (T::*pHandler)(void*, P))
		{
			// See if we already have a handler by the same signature
			Functor<T, P> temp(pTarget, pHandler);
			if(GetHandler(temp.GetSignature()))
				return;

			AddHandler(new Functor<T, P>(pTarget, pHandler));
		}

		/**
		 * Adds an event handler.  Does not allow duplicates.
		 */
		void AddHandler(IFunctor<P>* pFunctor)
		{
			if(GetHandler(pFunctor->GetSignature()))
				return;

			mHandlers.push_back(pFunctor);
		}
		
		/**
		 * Removes an event handler with the provided target and function pointer.
		 */
		template <class T>
		void RemoveHandler(T* pTarget, void (T::*fp)(void*, P))
		{
			Functor<T, P> temp(pTarget, fp);
			
			IFunctor<P>* pFunctor = NULL;
			while(pFunctor = RemoveHandlerBySignature(temp.GetSignature()))
				delete pFunctor;
		}

		/**
		 * Removes an event handler by signature.  Does not free the functor's memory.
		 */
		IFunctor<P>* RemoveHandlerBySignature(const Signature& signature)
		{
			for(uint32 i = 0; i < mHandlers.size(); i++)
			{
				IFunctor<P>* pFunctor = mHandlers[i];
				if(pFunctor->GetSignature() == signature)
				{
					mHandlers.erase(i);
					return pFunctor;
				}
			}

			return NULL;
		}

		/**
		 * Retrieves a handler by signature
		 */
		IFunctor<P>* GetHandler(const Signature& signature)
		{
			for(uint32 i = 0; i < mHandlers.size(); i++)
			{
				IFunctor<P>* pFunctor = mHandlers[i];
				if(pFunctor->GetSignature() == signature)
					return pFunctor;
			}	

			return NULL;
		}

		/**
		 * Clears all of the event handlers
		 */
		void Clear()
		{
			mHandlers.clear(true);
		}
		
		/**
		 * Executes all Handlers in the set with the provided sender object and 
		 * context as parameters.
		 */
		void operator()(void* pSender, P param)
		{			
			Array<IFunctor<P>*> temp(mHandlers);			
			for(uint32 i = 0; i < temp.size(); i++)
			{
				IFunctor<P>* pFunctor = temp[i];

				for(uint32 j = 0; j < mHandlers.size(); j++)
				{
					if(mHandlers[j] == pFunctor)
						(*pFunctor)(pSender, param);
				}				
			}
		}
		
	private:
		/**
		 * Array of event handlers.
		 */
		Array<IFunctor<P>*> mHandlers;
	};
}; // namespace Otter