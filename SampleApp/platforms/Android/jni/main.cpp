//
// Copyright (c) Aonyx Software, LLC
// All rights reserved.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
// CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

#include <stdlib.h>
#include <stdio.h>
#include <jni.h>
#include <android/log.h>
#include <GLES/gl.h>
#include <GLES/glext.h>

#include "Otter.h"
#include "Renderers/OGLESRenderer.h"
#include "FileSystems/SampleFileSystem.h"
#include "SoundSystems/AndroidSoundSystem.h"
#include "SampleUI.h"

SampleUI*				gSampleUI = NULL;
SampleFileSystem*		gGUIFileSys = NULL;
OGLESRenderer*			gGUIRenderer = NULL;
AndroidSoundSystem*		gGUISoundSystem = NULL;

int						gActualWidth = 480;
int						gActualHeight = 320;
int						gWidth = 480;
int						gHeight = 320;

extern "C"
{
/* Call to initialize the graphics state */
	JNIEXPORT void JNICALL Java_com_aonyx_ottersample_OtterRenderer_nativeInit( JNIEnv* env, jobject thiz)
	{
		gGUIFileSys = new SampleFileSystem();
		gGUIRenderer = new OGLESRenderer(gWidth, gHeight);
		gGUISoundSystem = new AndroidSoundSystem(env);

		gSampleUI = new SampleUI(gGUIRenderer, gGUISoundSystem, gGUIFileSys);
	}

	JNIEXPORT void JNICALL Java_com_aonyx_ottersample_OtterRenderer_nativeResize( JNIEnv*  env, jobject  thiz, jint w, jint h )
	{
		gActualWidth = w;
		gActualHeight = h;

		glViewport(0, 0, gActualWidth, gActualHeight);

		// The UI was built for 480x320.  We assume the height to remain the same,
		// and adjust the width accordingly.

		gWidth 	= gHeight * ((float)gActualWidth / (float)gActualHeight);
		gHeight = 320;

		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Otter Resolution: %d x %d", gWidth, gHeight);

		if(gGUIRenderer)
			gGUIRenderer->SetResolution(gWidth, gHeight);

		if(gSampleUI)
			gSampleUI->GetSystem()->SetResolution(gWidth, gHeight);
	}
	
	/* Call to finalize the graphics state */
	JNIEXPORT void JNICALL Java_com_aonyx_ottersample_OtterRenderer_nativeDone( JNIEnv*  env )
	{
		if(!gSampleUI)
			return;

		delete gSampleUI;
		delete gGUISoundSystem;
		delete gGUIRenderer;
		delete gGUIFileSys;

		gGUIFileSys = NULL;
		gGUIRenderer = NULL;
		gGUISoundSystem = NULL;
		gSampleUI = NULL;
	}
	
	/* This is called to indicate to the render loop that it should
	 * stop as soon as possible.
	 */
	JNIEXPORT void JNICALL Java_com_aonyx_ottersample_OtterGLSurfaceView_nativePause( JNIEnv*  env )
	{
		if(!gSampleUI)
			return;
	}
	
	/* Call to render the next GL frame */
	JNIEXPORT void JNICALL Java_com_aonyx_ottersample_OtterRenderer_nativeRender( JNIEnv*  env )
	{
		if(!gSampleUI)
			return;

		glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		if(gSampleUI)
		{
			gSampleUI->Update(1.0f);
			gSampleUI->Draw();
		}
	}

	/* Call to process a touch down*/
	JNIEXPORT void JNICALL Java_com_aonyx_ottersample_OtterRenderer_nativeTouchDown( JNIEnv*  env, jobject  thiz, float x, float y)
	{
		if(!gSampleUI)
			return;

		// Adjust the coordinates since we scaled to a new resolution
		x *= (gWidth / (float)gActualWidth);
		y *= (gHeight / (float)gActualHeight);

		Point point(x, y);
		if(gSampleUI)
		{
			__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Touch Down: (%0.2f, %0.2f)", x, y);
			gSampleUI->GetSystem()->OnPointsDown(&point, 1);
		}
	}

	/* Call to process a touch up*/
	JNIEXPORT void JNICALL Java_com_aonyx_ottersample_OtterRenderer_nativeTouchUp( JNIEnv*  env, jobject  thiz, float x, float y)
	{
		if(!gSampleUI)
			return;

		// Adjust the coordinates since we scaled to a new resolution
		x *= (gWidth / (float)gActualWidth);
		y *= (gHeight / (float)gActualHeight);

		Point point(x, y);
		if(gSampleUI)
		{
			__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Touch Up: (%0.2f, %0.2f)", x, y);
			gSampleUI->GetSystem()->OnPointsUp(&point, 1);
		}
	}

	/* Call to process a touch move*/
	JNIEXPORT void JNICALL Java_com_aonyx_ottersample_OtterRenderer_nativeTouchMove( JNIEnv*  env, jobject  thiz, float x, float y)
	{
		if(!gSampleUI)
			return;

		// Adjust the coordinates since we scaled to a new resolution
		x *= (gWidth / (float)gActualWidth);
		y *= (gHeight / (float)gActualHeight);

		Point point(x, y);
		if(gSampleUI)
		{
			__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Touch Move: (%0.2f, %0.2f)", x, y);
			gSampleUI->GetSystem()->OnPointsMove(&point, 1);
		}
	}
}
