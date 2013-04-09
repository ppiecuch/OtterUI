//
//  SampleAppViewController.m
//  SampleApp
//
//  Created by Lloyd Tullues on 12/30/10.
//  Copyright 2010 Ironblight Software, LLC. All rights reserved.
//

#import <QuartzCore/QuartzCore.h>

#import "SampleAppViewController.h"
#import "EAGLView.h"

#include "Otter.h"
#include "OGLESRenderer.h"
#include "iOSSoundSystem.h"
#include "SampleFileSystem.h"
#include "SampleUI.h"

@interface SampleAppViewController ()
@property (nonatomic, retain) EAGLContext *context;
@property (nonatomic, assign) CADisplayLink *displayLink;
@end

@implementation SampleAppViewController

@synthesize animating, context, displayLink;

- (void)awakeFromNib
{
    EAGLContext *aContext = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES1];
    
    if (!aContext)
        NSLog(@"Failed to create ES context");
    else if (![EAGLContext setCurrentContext:aContext])
        NSLog(@"Failed to set ES context current");
    
	self.context = aContext;
	[aContext release];
	
    [(EAGLView *)self.view setContext:context];
    [(EAGLView *)self.view setFramebuffer];
    
    animating = FALSE;
    animationFrameInterval = 1;
    self.displayLink = nil;
}

- (void)dealloc
{
    // Tear down context.
    if ([EAGLContext currentContext] == context)
        [EAGLContext setCurrentContext:nil];
    
    [context release];
    
    [super dealloc];
}

- (void)viewWillAppear:(BOOL)animated
{			
	int w = self.view.frame.size.width;
	int h = self.view.frame.size.height;
	
	mUIWidth = 320 * (h/(float)w);
	mUIHeight = 320;
	
	mGUIFileSys = new SampleFileSystem();
	mGUIRenderer = new OGLESRenderer(mUIWidth, mUIHeight);
	mGUISoundSys = new iOSSoundSystem();
	mSampleUI = new SampleUI(mGUIRenderer, mGUISoundSys, mGUIFileSys);
	mSampleUI->GetSystem()->SetResolution(mUIWidth, mUIHeight);
	
    [self startAnimation];
    
    [super viewWillAppear:animated];
}

- (void)viewWillDisappear:(BOOL)animated
{
    [self stopAnimation];
	
	delete mSampleUI;	
	delete mGUISoundSys;
	delete mGUIRenderer;
	delete mGUIFileSys;
	
	mSampleUI = NULL;
	mGUIRenderer = NULL;
	mGUIFileSys = NULL;
    
    [super viewWillDisappear:animated];
}

- (void)viewDidUnload
{
	[super viewDidUnload];
	
    // Tear down context.
    if ([EAGLContext currentContext] == context)
        [EAGLContext setCurrentContext:nil];
	
	self.context = nil;	
}

- (NSInteger)animationFrameInterval
{
    return animationFrameInterval;
}

- (void)setAnimationFrameInterval:(NSInteger)frameInterval
{
    /*
	 Frame interval defines how many display frames must pass between each time the display link fires.
	 The display link will only fire 30 times a second when the frame internal is two on a display that refreshes 60 times a second. The default frame interval setting of one will fire 60 times a second when the display refreshes at 60 times a second. A frame interval setting of less than one results in undefined behavior.
	 */
    if (frameInterval >= 1)
    {
        animationFrameInterval = frameInterval;
        
        if (animating)
        {
            [self stopAnimation];
            [self startAnimation];
        }
    }
}

- (void)startAnimation
{
    if (!animating)
    {
        CADisplayLink *aDisplayLink = [CADisplayLink displayLinkWithTarget:self selector:@selector(drawFrame)];
        [aDisplayLink setFrameInterval:animationFrameInterval];
        [aDisplayLink addToRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
        self.displayLink = aDisplayLink;
        
        animating = TRUE;
    }
}

- (void)stopAnimation
{
    if (animating)
    {
        [self.displayLink invalidate];
        self.displayLink = nil;
        animating = FALSE;
    }
}

- (void)drawFrame
{	
    [(EAGLView *)self.view setFramebuffer];
	    
    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    glClear(GL_COLOR_BUFFER_BIT |  GL_DEPTH_BUFFER_BIT);
	glViewport(0, 0, [(EAGLView *)self.view framebufferWidth], [(EAGLView *)self.view framebufferHeight]);
	
	if(mSampleUI)
	{
		mSampleUI->Update(1.0f);
		mSampleUI->Draw();
	}
    
    [(EAGLView *)self.view presentFramebuffer];
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc. that aren't in use.
}

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
	int w = self.view.frame.size.width;
	int h = self.view.frame.size.height;
	
	int numPoints = [touches count];
	Otter::Point* points = new Otter::Point[numPoints * 2];
	
	int i = 0;
	for(UITouch* t in touches)
	{		
		CGPoint point = [t locationInView:self.view];						
		points[i].x = point.x * (mUIWidth/(float)h);
		points[i].y = point.y * (mUIHeight/(float)w);
		
		i++;
	}
	
	if(mSampleUI)
	{
		mSampleUI->GetSystem()->OnPointsDown (points, numPoints);	
	}
	
	delete[] points;
}

- (void)touchesMoved:(NSSet *)touches withEvent:(UIEvent *)event
{		
	int w = self.view.frame.size.width;
	int h = self.view.frame.size.height;
	
	int numPoints = [touches count];
	Otter::Point* points = new Otter::Point[numPoints * 2];
	
	int i = 0;
	for(UITouch* t in touches)
	{		
		CGPoint point = [t locationInView:self.view];						
		points[i].x = point.x * (mUIWidth/(float)h);
		points[i].y = point.y * (mUIHeight/(float)w);
		
		i++;
	}
	
	if(mSampleUI)
		mSampleUI->GetSystem()->OnPointsMove(points, numPoints);
	
	delete[] points;
}

- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event
{
	int w = self.view.frame.size.width;
	int h = self.view.frame.size.height;
	
	int numPoints = [touches count];
	Otter::Point* points = new Otter::Point[numPoints * 2];
	
	int i = 0;
	for(UITouch* t in touches)
	{		
		CGPoint point = [t locationInView:self.view];				
		points[i].x = point.x * (mUIWidth/(float)h);
		points[i].y = point.y * (mUIHeight/(float)w);
		
		i++;
	}
	
	if(mSampleUI)
		mSampleUI->GetSystem()->OnPointsUp(points, numPoints);
	
	delete[] points;
}

@end
