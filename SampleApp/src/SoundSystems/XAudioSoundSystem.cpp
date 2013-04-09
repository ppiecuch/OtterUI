#include "XAudioSoundSystem.h"
#include "SDKwavefile.h"
#include <strsafe.h>
#include <windows.h>
#include <xaudio2.h>
#include <strsafe.h>
#include <shellapi.h>
#include <mmsystem.h>
#include <conio.h>

//--------------------------------------------------------------------------------------
// Helper macros
//--------------------------------------------------------------------------------------
#ifndef SAFE_DELETE_ARRAY
#define SAFE_DELETE_ARRAY(p) { if(p) { delete[] (p);   (p)=NULL; } }
#endif
#ifndef SAFE_RELEASE
#define SAFE_RELEASE(p)      { if(p) { (p)->Release(); (p)=NULL; } }
#endif

/* Constructor
 */
XAudioSoundSystem::XAudioSoundSystem(void)
{
    HRESULT hr;
    mXAudio2 = NULL;	
	mMasteringVoice = NULL;

    UINT32 flags = 0;

	//
    // Initialize XAudio2
    //
    CoInitializeEx( NULL, COINIT_MULTITHREADED );

	if( FAILED( hr = XAudio2Create( &mXAudio2, flags ) ) )
    {
        wprintf( L"Failed to init XAudio2 engine: %#X\n", hr );
		return;
    }	

    //
    // Create a mastering voice
    //
    if( FAILED( hr = mXAudio2->CreateMasteringVoice( &mMasteringVoice ) ) )
    {
        wprintf( L"Failed creating mastering voice: %#X\n", hr );
        SAFE_RELEASE( mXAudio2 );
		return;
    }
}

/* Virtual Destructor
 */
XAudioSoundSystem::~XAudioSoundSystem(void)
{
	std::map<int, Sound*>::iterator it = mSounds.begin();
	for(; it != mSounds.end(); it++)
	{
		Sound* pSound = it->second;

		if(pSound)
		{
			pSound->mVoice->Stop();
			pSound->mVoice->DestroyVoice();

			SAFE_DELETE_ARRAY( pSound->mData );
			delete pSound;
		}
	}

	mSounds.clear();

	if(mMasteringVoice)
		mMasteringVoice->DestroyVoice();

    if(mXAudio2)
		SAFE_RELEASE( mXAudio2 );

	mMasteringVoice = NULL;
	mXAudio2 = NULL;

    CoUninitialize();
}

/* Loads a sound with the specified id and path
 */
void XAudioSoundSystem::OnLoadSound(uint32 soundID, const char* szSoundPath)
{
	if(mSounds[soundID] != NULL)
		return;

	HRESULT hr = S_OK;

	char szFullPath[1024];
	sprintf_s(szFullPath, 1024, "Data/Win32/%s", szSoundPath);
		
    size_t convertedChars = 0;
    wchar_t wpath[1024];
	mbstowcs_s(&convertedChars, wpath, strlen(szFullPath) + 1, szFullPath, _TRUNCATE);

    //
    // Read in the wave file
    //
    CWaveFile wav;
    if( FAILED( hr = wav.Open( wpath, NULL, WAVEFILE_READ ) ) )
    {
        wprintf( L"Failed reading WAV file: %#X (%s)\n", hr, wpath );
        return;
    }

    // Get format of wave file
    WAVEFORMATEX* pwfx = wav.GetFormat();

    // Calculate how many bytes and samples are in the wave
    DWORD cbWaveSize = wav.GetSize();

    // Read the sample data into memory
    BYTE* pbWaveData = new BYTE[ cbWaveSize ];

    if( FAILED( hr = wav.Read( pbWaveData, cbWaveSize, &cbWaveSize ) ) )
    {
        wprintf( L"Failed to read WAV data: %#X\n", hr );
        SAFE_DELETE_ARRAY( pbWaveData );
        return;
    }

    //
    // Play the wave using a XAudio2SourceVoice
    //

    // Create the source voice
    IXAudio2SourceVoice* pSourceVoice;
    if( FAILED( hr = mXAudio2->CreateSourceVoice( &pSourceVoice, pwfx ) ) )
    {
        wprintf( L"Error %#X creating source voice\n", hr );
        SAFE_DELETE_ARRAY( pbWaveData );
        return;
    }

    // Submit the wave sample data using an XAUDIO2_BUFFER structure
    XAUDIO2_BUFFER buffer = {0};
    buffer.pAudioData = pbWaveData;
    buffer.Flags = XAUDIO2_END_OF_STREAM;  // tell the source voice not to expect any data after this buffer
    buffer.AudioBytes = cbWaveSize;

    if( FAILED( hr = pSourceVoice->SubmitSourceBuffer( &buffer ) ) )
    {
        wprintf( L"Error %#X submitting source buffer\n", hr );
        pSourceVoice->DestroyVoice();
        SAFE_DELETE_ARRAY( pbWaveData );
        return;
    }

	Sound* pSound = new Sound();
	pSound->mVoice = pSourceVoice;
	pSound->mAudioBuffer = buffer;
	pSound->mData = pbWaveData;
	pSound->mStarted = false;

	mSounds[soundID] = pSound;
}	

/* Unloads a sound with the specified id
 */
void XAudioSoundSystem::OnUnloadSound(uint32 soundID)
{
	Sound* pSound = mSounds[soundID];

	if(pSound == NULL)
		return;

	pSound->mVoice->Stop();
	pSound->mVoice->DestroyVoice();

	SAFE_DELETE_ARRAY( pSound->mData );
	delete pSound;

	mSounds[soundID] = NULL;
}

/* Plays a sound at the specified volume
 */
void XAudioSoundSystem::OnPlaySound(uint32 soundID, float volume)
{
	Sound* pSound = mSounds[soundID];

	if(pSound == NULL)
		return;

	if(pSound->mStarted)
	{
		pSound->mVoice->Stop();	
		pSound->mVoice->FlushSourceBuffers();
		pSound->mVoice->SubmitSourceBuffer( &pSound->mAudioBuffer );
	}

	pSound->mVoice->Start();
	pSound->mStarted = true;
} 