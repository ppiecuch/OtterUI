// SampleApp.cpp : Defines the entry point for the application.
//
#include "SampleApp.h"

#include "Otter.h"

#include "SampleUI.h"
#include "Renderers/D3DRenderer.h"
#include "FileSystems/SampleFileSystem.h"
#include "SoundSystems/XAudioSoundSystem.h"

SampleUI*			gSampleUI		= NULL;
D3DRenderer*		gRenderer		= NULL;
SampleFileSystem*	gFileSystem		= NULL;
XAudioSoundSystem*	gSoundSystem	= NULL;

int					gScreenWidth		= 960;
int					gScreenHeight		= 640;

#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE hInst;								// current instance
TCHAR szTitle[MAX_LOADSTRING];					// The title bar text
TCHAR szWindowClass[MAX_LOADSTRING];			// the main window class name

// Forward declarations of functions included in this code module:
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int, int, int, HWND&);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

int APIENTRY _tWinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPTSTR    lpCmdLine,
                     int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

 	// TODO: Place code here.
	MSG msg;
	HWND hWnd;

	// Initialize global strings
	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadString(hInstance, IDC_SAMPLEAPP, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance);

	// Perform application initialization:
	if (!InitInstance (hInstance, nCmdShow, gScreenWidth, gScreenHeight, hWnd))
		return FALSE;

	gRenderer = new D3DRenderer(hWnd, gScreenWidth, gScreenHeight, false);
	gFileSystem = new SampleFileSystem();
	gSoundSystem = new XAudioSoundSystem();
	gSampleUI = new SampleUI(gRenderer, gSoundSystem, gFileSystem);

	if(gSampleUI)
		gSampleUI->GetSystem()->SetResolution(gScreenWidth, gScreenHeight);

	DWORD curTime = timeGetTime();
	DWORD oldTime = curTime;

	// Main message loop:
	while (1)
	{
		curTime = timeGetTime();

		// Very rudimentary throttling mechanism.  Approx 30fps.
		if((curTime - oldTime) < 33)
			continue;

		// Perform Windows message handling
		if(PeekMessage(&msg,NULL,0,0,PM_REMOVE))
		{
			if (msg.message == WM_QUIT) 
				break;

			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}

		// Update the GUI System, and draw.
		// The value to 'update' is the number of frames to process in the GUI system,
		// allowing you to dictate how many frames per second of animation.
		if(gSampleUI)
		{
			gSampleUI->Update(((curTime - oldTime) / 1000.0f) * 30.0f);
			gSampleUI->Draw();
		}

		oldTime = curTime;
	}
	
	delete gSampleUI;
	delete gSoundSystem;
	delete gRenderer;
	delete gFileSystem;

	gFileSystem = NULL;
	gRenderer = NULL;	
	gSoundSystem = NULL;
	gSampleUI = NULL;

	return (int) msg.wParam;
}

//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
//  COMMENTS:
//
//    This function and its usage are only necessary if you want this code
//    to be compatible with Win32 systems prior to the 'RegisterClassEx'
//    function that was added to Windows 95. It is important to call this function
//    so that the application will get 'well formed' small icons associated
//    with it.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
	WNDCLASSEX wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style			= CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc	= WndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hInstance;
	wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_SAMPLEAPP));
	wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
	wcex.lpszMenuName	= MAKEINTRESOURCE(IDC_SAMPLEAPP);
	wcex.lpszClassName	= szWindowClass;
	wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassEx(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow, int width, int height, HWND& hWnd)
{
   hInst = hInstance; // Store instance handle in our global variable

   hWnd = CreateWindow(	szWindowClass, 
						szTitle, 
						WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU,
						GetSystemMetrics(SM_CXSCREEN) / 2 - width / 2, 
						GetSystemMetrics(SM_CYSCREEN) / 2 - height / 2, 
						width + GetSystemMetrics(SM_CXFIXEDFRAME) * 2, 
						height + GetSystemMetrics(SM_CYFIXEDFRAME) * 2 + GetSystemMetrics(SM_CYCAPTION),
						NULL, 
						NULL,
						hInstance, 
						NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   return TRUE;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the main window.
//
//  WM_PAINT	- Paint the main window
//  WM_DESTROY	- post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	Otter::Point point;

	switch (message)
	{
	case WM_PAINT:
		ValidateRect(hWnd, NULL);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	case WM_MOUSEMOVE:
		point.x = (float)((lParam) & 0xFFFF);
		point.y = (float)((lParam >> 16) & 0xFFFF);

		if(gSampleUI)
			gSampleUI->GetSystem()->OnPointsMove(&point, 1);
		break;
	case WM_LBUTTONDOWN:
		point.x = (float)((lParam) & 0xFFFF);
		point.y = (float)((lParam >> 16) & 0xFFFF);

		if(gSampleUI)
			gSampleUI->GetSystem()->OnPointsDown(&point, 1);
		break;
	case WM_LBUTTONUP:
		point.x = (float)((lParam) & 0xFFFF);
		point.y = (float)((lParam >> 16) & 0xFFFF);

		if(gSampleUI)
			gSampleUI->GetSystem()->OnPointsUp(&point, 1);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}