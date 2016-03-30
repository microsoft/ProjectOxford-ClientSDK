#include "TargetConditionals.h"

#if !TARGET_OS_IPHONE

#import <Cocoa/Cocoa.h> 

#define UNIVERSAL_RESPONDER         NSResponder <NSApplicationDelegate>
#define UNIVERSAL_WINDOW            NSWindow
#define UNIVERSAL_APPLICATION       NSApplication
#define UNIVERSAL_VIEWCONTROLLER    NSViewController
#define UNIVERSAL_TEXTVIEW          NSTextView
#define UNIVERSAL_BUTTON            NSButton
#define UNIVERSAL_VIEW              NSView
#define UNIVERSAL_APP_ARGV          const char *

#define UNIVERSAL_APP_MAIN(__argc, __argv, __class)  \
    NSApplicationMain(__argc, __argv)

#define UNIVERSAL_TEXTVIEW_SETTEXT(__textView, __text)  \
    [__textView setString: __text]

#define UNIVERSAL_BUTTON_SETCHECKED(__buttonView, __checked)  \
    [__buttonView setState: (__checked) ? NSOnState : NSOffState]

#define UNIVERSAL_BUTTON_GETCHECKED(__buttonView)  \
    (__buttonView.state == NSOnState)

#else

#import <UIKit/UIKit.h> 

#define UNIVERSAL_RESPONDER         UIResponder <UIApplicationDelegate>
#define UNIVERSAL_WINDOW            UIWindow
#define UNIVERSAL_APPLICATION       UIApplication
#define UNIVERSAL_VIEWCONTROLLER    UIViewController
#define UNIVERSAL_TEXTVIEW          UITextView
#define UNIVERSAL_BUTTON            UIButton
#define UNIVERSAL_VIEW              UIView
#define UNIVERSAL_APP_ARGV          char *

#define UNIVERSAL_APP_MAIN(__argc, __argv, __class)  \
    UIApplicationMain(__argc, __argv, nil, __class)

#define UNIVERSAL_TEXTVIEW_SETTEXT(__textView, __text)  \
    __textView.text = text

#define UNIVERSAL_BUTTON_SETCHECKED(__buttonView, __checked)  \
    [__buttonView setSelected: __checked]

#define UNIVERSAL_BUTTON_GETCHECKED(__buttonView)  \
    __buttonView.selected

#endif 

#import <Foundation/Foundation.h>
#import <SpeechSDK/SpeechRecognitionService.h>

#import "AppDelegate.h"
#import "ViewController.h"
