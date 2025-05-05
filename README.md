<div align="center">
  <img src="https://github.com/plxl/mpegui/blob/main/icon.png" height="100">
  
  ## mpegui ##

  The FFmpeg GUI that does specifically what I want it to do.
  Written in C# .NET for Windows.
</div>

### Why Not Handbrake? ###
I made this because I wanted to have a nice easy GUI where I can drag and drop my beat saber clips and apply audio gain, delay, a crop filter, with everything right in front of me. No tinkering with a massive selection of options. Just everything I need.

<img src="https://github.com/plxl/mpegui/blob/main/images/main_window.jpg" height="450"> 


### Features ###
- Drag-and-drop support
- Audio gain and delay control
- Crop and crop presets (1:1 and 16:9-9:16)
- Trim video (start and end, as well as a feature to automatically get the end)
- Set encoder from a list or type manually
- Add hvc1 tag for x265/HEVC videos so they play on Apple devices
- Output filename customisation
  - {filename} will be replaced with the original filename
  - {filename-3} trims the last 3 characters from the original filename (if you wanted to change the extension, for example)
  - "{filename-3}mov" would change "video.mp4" to "video.mov"
- Copy and Paste your settings from one to all selected videos in the list
- Copy the run command for the current video if you want to run it manually
- Options for default configurations
  - Default encoder
  - Always automatically set the hvc1 tag for Apple support on x265 encoder

### Requirements ###
- .NET Framework 4.8
- FFmpeg installed and added to PATH
