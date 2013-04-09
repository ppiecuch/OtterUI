//
//  SampleAppAppDelegate.h
//  SampleApp
//
//  Created by Lloyd Tullues on 12/30/10.
//  Copyright 2010 Ironblight Software, LLC. All rights reserved.
//

#import <UIKit/UIKit.h>

@class SampleAppViewController;

@interface SampleAppAppDelegate : NSObject <UIApplicationDelegate> {
    UIWindow *window;
    SampleAppViewController *viewController;
}

@property (nonatomic, retain) IBOutlet UIWindow *window;
@property (nonatomic, retain) IBOutlet SampleAppViewController *viewController;

@end

