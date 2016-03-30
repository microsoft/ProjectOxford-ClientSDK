Pod::Spec.new do |s|
  s.name             = "ProjectOxfordFace"
  s.version          = "1.0"
  s.summary          = "Project Oxford Face iOS SDK"

  s.platform     = :ios, '9.0'
  s.requires_arc = true

  s.source_files = 'Pod/Classes/**/*'
  s.resource_bundles = {
    'ProjectOxfordFace' => ['Pod/Assets/*.png']
  }
end
