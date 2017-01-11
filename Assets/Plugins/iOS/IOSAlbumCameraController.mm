//
//  IOSAlbumCameraController.m
//  Unity-iPhone
//
//  Created by gamai on 16/7/27.
//
//

#import "IOSAlbumCameraController.h"
#import "Pingpp.h"

#define kUrl            @"http://orderapi.bi-lang.com/ordersys/order/order/pingCharge"
@implementation IOSAlbumCameraController

- (void)showActionSheet
{
    NSLog(@" --- showActionSheet !!");
    
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:nil message:nil preferredStyle:UIAlertControllerStyleActionSheet];
    
    UIAlertAction *albumAction = [UIAlertAction actionWithTitle:@"相册" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        NSLog(@"click album action!");
        [self showPicker:UIImagePickerControllerSourceTypePhotoLibrary allowsEditing:YES];
    }];
    
    UIAlertAction *cameraAction = [UIAlertAction actionWithTitle:@"相机" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        NSLog(@"click camera action!");
        [self showPicker:UIImagePickerControllerSourceTypeCamera allowsEditing:YES];
    }];
    
    UIAlertAction *album_cameraAction = [UIAlertAction actionWithTitle:@"相册&相机" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        NSLog(@"click album&camera action!");
        //[self showPicker:UIImagePickerControllerSourceTypeCamera];
        [self showPicker:UIImagePickerControllerSourceTypeSavedPhotosAlbum allowsEditing:YES];
    }];
    
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"取消" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        NSLog(@"click cancel action!");
    }];
    
    
    [alertController addAction:albumAction];
    [alertController addAction:cameraAction];
    [alertController addAction:album_cameraAction];
    [alertController addAction:cancelAction];
    
    UIViewController *vc = UnityGetGLViewController();
    [vc presentViewController:alertController animated:YES completion:^{
        NSLog(@"showActionSheet -- completion");
    }];
}

- (void)showPicker:
(UIImagePickerControllerSourceType)type
     allowsEditing:(BOOL)flag
{
    NSLog(@" --- showPicker!!");
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    picker.sourceType = type;
    picker.allowsEditing = flag;
    
    [self presentViewController:picker animated:YES completion:nil];
}

// 打开相册后选择照片时的响应方法
- (void)imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary*)info
{
    NSLog(@" --- imagePickerController didFinishPickingMediaWithInfo!!");
    // Grab the image and write it to disk
    UIImage *image;
    UIImage *image2;
    image = [info objectForKey:UIImagePickerControllerEditedImage];
    UIGraphicsBeginImageContext(CGSizeMake(256,256));
    [image drawInRect:CGRectMake(0, 0, 256, 256)];
    image2 = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    // 得到了image，然后用你的函数传回u3d
    NSData *imgData;
    if(UIImagePNGRepresentation(image2) == nil)
    {
        NSLog(@" --- actionSheet slse!! 11 ");
        imgData= UIImageJPEGRepresentation(image, .6);
    }
    else
    {
        NSLog(@" --- actionSheet slse!! 22 ");
        imgData= UIImagePNGRepresentation(image2);
    }
    
    NSString *_encodeImageStr = [imgData base64Encoding];
    //UnitySendMessage( "Start", "OpenCameraOrAblumCallBack", _encodeImageStr.UTF8String);
    NSString *_string = [@"{\"function\":\"PhotoAlbumCallBack\",\"fileName\":\"" stringByAppendingString:_encodeImageStr];
    NSString *_string2 = [_string stringByAppendingString:@"\"}"];
    UnitySendMessage("Start", "SdkCall",_string2.UTF8String);
    
    // 关闭相册
    [picker dismissViewControllerAnimated:YES completion:nil];
}

// 打开相册后点击“取消”的响应
- (void)imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
    NSLog(@" --- imagePickerControllerDidCancel !!");
    [self dismissViewControllerAnimated:YES completion:nil];
}

+(void) saveImageToPhotosAlbum:(NSString*) readAdr
{
    //NSLog(@"readAdr: ");
    //NSLog(readAdr);
    UIImage* image = [UIImage imageWithContentsOfFile:readAdr];
    UIImageWriteToSavedPhotosAlbum(image,
                                   self,
                                   @selector(image:didFinishSavingWithError:contextInfo:),
                                   NULL);
}

+(void) image:(UIImage*)image didFinishSavingWithError:(NSError*)error contextInfo:(void*)contextInfo
{
    NSString* result;
    if(error)
    {
        result = @"图片保存到相册失败!";
    }
    else
    {
        result = @"图片保存到相册成功!";
    }
    //UnitySendMessage( "Start", "OpenCameraOrAblumCallBack", result.UTF8String);
}

+(UIViewController *) getCurrentRootViewController {
    UIViewController *result;
    UIWindow *topWindow = [[UIApplication sharedApplication] keyWindow];
    if (topWindow.windowLevel != UIWindowLevelNormal) {
        NSArray *windows = [[UIApplication sharedApplication] windows];
        for(topWindow in windows) {
            if (topWindow.windowLevel == UIWindowLevelNormal)
                break;
        }
    }
    UIView *rootView = [[topWindow subviews] objectAtIndex:0];
    id nextResponder = [rootView nextResponder];
    if ([nextResponder isKindOfClass:[UIViewController class]]) {
        result = nextResponder;
    } else if ([topWindow respondsToSelector:@selector(rootViewController)]
               && topWindow.rootViewController != nil) {
        result = topWindow.rootViewController;
    } else {
        return nil;
    }
    return result;
}
@end

//------------- called by unity -----begin-----------------
#if defined (__cplusplus)
extern "C" {
#endif
    
    // 弹出一个菜单项：相册、相机
    void _showActionSheet()
    {
        NSLog(@" -unity call-- _showActionSheet !!");
        IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
        UIViewController *vc = UnityGetGLViewController();
        [vc.view addSubview: app.view];
        
        [app showActionSheet];
    }
    
    // 打开相册
    void _iosOpenPhotoLibrary()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypePhotoLibrary])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypePhotoLibrary allowsEditing:NO];
        }
        else
        {
            UnitySendMessage("Start", "SdkCall","{\"function\":\"PhotoAlbumCallBack\",\"fileName\":\"image.jpg\"}");
        }
    }
    
    // 打开相册
    void _iosOpenPhotoAlbums()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeSavedPhotosAlbum])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypeSavedPhotosAlbum allowsEditing:NO];
        }
        else
        {
            _iosOpenPhotoLibrary();
        }
    }
    
    // 打开相机
    void _iosOpenCamera()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypeCamera allowsEditing:NO];
        }
        else
        {
            UnitySendMessage("Start", "SdkCall","{\"function\":\"PhotoAlbumCallBack\",\"fileName\":\"image.jpg\"}");
        }
    }
    
    
    // 打开相册--可编辑
    void _iosOpenPhotoLibrary_allowsEditing()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypePhotoLibrary])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypePhotoLibrary allowsEditing:YES];
        }
        else
        {
            UnitySendMessage("Start", "SdkCall","{\"function\":\"PhotoAlbumCallBack\",\"fileName\":\"image.jpg\"}");
        }
        
    }
    
    // 打开相册--可编辑
    void _iosOpenPhotoAlbums_allowsEditing()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeSavedPhotosAlbum])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypeSavedPhotosAlbum allowsEditing:YES];
        }
        else
        {
            _iosOpenPhotoLibrary();
        }
        
    }
    
    // 打开相机--可编辑
    void _iosOpenCamera_allowsEditing()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypeCamera allowsEditing:YES];
        }
        else
        {
            UnitySendMessage("Start", "SdkCall","{\"function\":\"PhotoAlbumCallBack\",\"fileName\":\"image.jpg\"}");
        }
    }
    
    void _iosSaveImageToPhotosAlbum(char* readAddr)
    {
        NSString* temp = [NSString stringWithUTF8String:readAddr];
        [IOSAlbumCameraController saveImageToPhotosAlbum:temp];
    }
    
    void _iosCall(const char *number){
        NSString *allString = [NSString stringWithFormat:@"tel:4006880988"];
        //NSString *allString =[[NSString alloc] initWithCString:(const char*)number encoding:NSUTF8StringEncoding];
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:allString]];
    }
    
    void _ReservationModification(const char *orderNo, const char *subject, const char *body, const char *amount, const char *channel, const char *token){
        NSString *string_orderNo = [[NSString alloc] initWithCString:(const char*)orderNo encoding:NSUTF8StringEncoding];
        NSString *string_subject = [[NSString alloc] initWithCString:(const char*)subject encoding:NSUTF8StringEncoding];
        NSString *string_body = [[NSString alloc] initWithCString:(const char*)body encoding:NSUTF8StringEncoding];
        NSString *string_mon = [[NSString alloc] initWithCString:(const char*)amount encoding:NSUTF8StringEncoding];
        NSString *string_channel = [[NSString alloc] initWithCString:(const char*)channel encoding:NSUTF8StringEncoding];
        NSString *string_token = [[NSString alloc] initWithCString:(const char*)token encoding:NSUTF8StringEncoding];
        
        NSURL* url = [NSURL URLWithString:kUrl];
        NSMutableURLRequest * postRequest=[NSMutableURLRequest requestWithURL:url];
        NSString *kUrlScheme = @"wx5e582a51161ac563";
        [Pingpp setAppId:@"app_OOW5eH44arP8yj5W"];
        NSDictionary* dict = @{
                               @"order_no" : string_orderNo,
                               @"subject" : string_subject,
                               @"body" :string_body,
                               @"amount"  : string_mon,
                               @"channel" : string_channel,
                               @"token" : string_token
                               };
        
        [postRequest setHTTPMethod:@"POST"];
        [postRequest setValue:@"application/json; charset=utf-8" forHTTPHeaderField:@"Content-Type"];
        
        NSData* data = [NSJSONSerialization dataWithJSONObject:dict options:NSJSONWritingPrettyPrinted error:nil];
        NSString *bodyData = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
        
        [postRequest setHTTPBody:[NSData dataWithBytes:[bodyData UTF8String] length:strlen([bodyData UTF8String])]];
        UIViewController * __weak weakSelf = [IOSAlbumCameraController  getCurrentRootViewController];
        
        NSOperationQueue *queue = [[NSOperationQueue alloc] init];
        [NSURLConnection sendAsynchronousRequest:postRequest queue:queue completionHandler:^(NSURLResponse *response, NSData *data, NSError *connectionError) {
            dispatch_async(dispatch_get_main_queue(), ^{
                NSHTTPURLResponse* httpResponse = (NSHTTPURLResponse*)response;
                if (httpResponse.statusCode != 200) {
                    return;
                }
                if (connectionError != nil) {
                    return;
                }
                NSString* charge = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
                [Pingpp createPayment:charge viewController:weakSelf appURLScheme:kUrlScheme withCompletion:^(NSString *result, PingppError *error) {
                    if (error == nil) {
                        UnitySendMessage("Start", "SdkCall", "{\"function\":\"PaySucceed\",\"result\":\"success\"}");
                    } else {
                        UnitySendMessage("Start", "SdkCall", "{\"function\":\"PayFailed\",\"result\":\"faile\"}");
                    }
                }];
            });
        }];
    }
    
#if defined (__cplusplus)
}
#endif
//------------- called by unity -----end-----------------Charge
