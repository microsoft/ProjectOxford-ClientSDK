Pod::Spec.new do |s|
  s.name             = "ProjectOxfordFace"
  s.version          = "1.0"
  s.summary          = "Microsoft Project Oxford Face iOS SDK"

  s.description  = <<-DESC
                   Integrate Microsoft Project Oxford Face APIs into your iOS App!
                   DESC
  s.homepage         = "https://github.com/Microsoft/ProjectOxford-ClientSDK"
  s.screenshots      = "https://raw.githubusercontent.com/Microsoft/ProjectOxford-ClientSDK/master/Face/iOS/SampleScreenshots/SampleScreenshot1.png", "https://raw.githubusercontent.com/Microsoft/ProjectOxford-ClientSDK/master/Face/iOS/SampleScreenshots/SampleScreenshot2.png", "https://raw.githubusercontent.com/Microsoft/ProjectOxford-ClientSDK/master/Face/iOS/SampleScreenshots/SampleScreenshot3.png", "https://raw.githubusercontent.com/Microsoft/ProjectOxford-ClientSDK/master/Face/iOS/SampleScreenshots/SampleScreenshot4.png"
  s.license          = 'MIT'
  s.author           = { "Project Oxford SDK" => "oxfordGithub@microsoft.com" }
  s.source           = { :git => "https://github.com/Microsoft/ProjectOxford-ClientSDK.git", :branch => "master" }
  s.platform     = :ios, '9.0'
  s.requires_arc = true

  s.source_files = 'Face/iOS/Pod/Classes/**/*'
  s.resource_bundles = {
    'ProjectOxfordFace' => ['Face/iOS/Pod/Assets/*.png']
  }
end
