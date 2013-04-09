//
//  SampleAppAppDelegate.m
//  SampleApp
//
//  Created by Lloyd Tullues on 12/30/10.
//  Copyright 2010 Ironblight Software, LLC. All rights reserved.
//

#import "SampleAppAppDelegate.h"
#import "SampleAppViewController.h"

@implementation SampleAppAppDelegate

@synthesize window;
@synthesize viewController;

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{	
	[application setStatusBarHidden:YES withAnimation:UIStatusBarAnimationNone];

	UIScreen *screen = [UIScreen mainScreen];
    [window setFrame:[screen applicationFrame]];
	
	[window makeKeyAndVisible];
	
	int w = window.frame.size.width;
	int h = window.frame.size.height;
	
	CGAffineTransform translate = CGAffineTransformMakeTranslation(w/2 - h/2, h/2 - w/2);
	CGAffineTransform rotate = CGAffineTransformMakeRotation(3.14159f / 2.0f);
	CGAffineTransform transform = CGAffineTransformConcat(rotate, translate);
	
	[self.viewController.view setFrame:CGRectMake(0, 0, h, w)];
	[self.viewController.view setTransform:transform];
	
    [self.window addSubview:self.viewController.view];
	
    return YES;
}

- (void)applicationWillResignActive:(UIApplication *)application
{
    [self.viewController stopAnimation];
}

- (void)applicationDidBecomeActive:(UIApplication *)application
{
    [self.viewController startAnimation];
}

- (void)applicationWillTerminate:(UIApplication *)application
{
    [self.viewController stopAnimation];
}

- (void)applicationDidEnterBackground:(UIApplication *)application
{
    // Handle any background procedures not related to animation here.
}

- (void)applicationWillEnterForeground:(UIApplication *)application
{
    // Handle any foreground procedures not related to animation here.
}

- (void)dealloc
{
    [viewController release];
    [window release];
    
    [super dealloc];
}

@end
