//
//  SampleAppViewController.h
//  SampleApp
//
//  Created by Lloyd Tullues on 12/30/10.
//  Copyright 2010 Ironblight Software, LLC. All rights reserved.
//

#import <UIKit/UIKit.h>

#import <OpenGLES/EAGL.h>

#import <OpenGLES/ES1/gl.h>
#import <OpenGLES/ES1/glext.h>
#import <OpenGLES/ES2/gl.h>
#import <OpenGLES/ES2/glext.h>

class SampleFileSystem;
class SampleUI;
class OGLESRenderer;
class iOSSoundSystem;

namespace Otter { class System; }

@interface SampleAppViewController : UIViewController
{
    EAGLContext *context;
    GLuint program;
    
    BOOL animating;
    NSInteger animationFrameInterval;
    CADisplayLink *displayLink;	
	
	SampleUI*			mSampleUI;
	SampleFileSystem*	mGUIFileSys;
	OGLESRenderer*		mGUIRenderer;
	iOSSoundSystem*		mGUISoundSys;
	
	int					mUIWidth;
	int					mUIHeight;
}

@property (readonly, nonatomic, getter=isAnimating) BOOL animating;
@property (nonatomic) NSInteger animationFrameInterval;

- (void)startAnimation;
- (void)stopAnimation;

@end
