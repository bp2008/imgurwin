# imgurwin

A Windows application that enables easy Imgur.com uploads and screen captures, and automates the creation of various sized thumbnail links to the full image.

ImgurWin was inspired by (*not affiliated* with) the program "MyImgur" from http://myimgur.eden.fm/.  The two programs share a very similar visual layout and core feature set.  ImgurWin was created to provide similar image uploading functionality and a more powerful link generator.

![ImgurWin](http://i.imgur.com/xAgKD6O.jpg)

## Image Sources

Images can be uploaded from a number of sources:

* Files on your computer (browse or drag & drop)
* Images on the clipboard
* Web URLs, either from the clipboard or from drag & drop (multiple URLs should be on separate lines)
* Draw a rectangle to snapshot a region of your screen, just like the Snipping Tool included in Windows.

## Bulk image uploads

ImgurWin can upload more than one image at a time, and when you copy a link, you get links to all the images you uploaded in the last operation.  You may also bulk-delete all images that were uploaded in the last operation, or delete images one at a time.

## Automated Link Creation

Part of what makes ImgurWin unique and useful is the powerful automated link creation.  This is one aspect where other applications, even the Imgur website itself, fall short.  I post images to forums all the time and I always found myself having to manually edit the image URL to make it point at the thumbnail size I wanted, and edit the link URL to point at the full image instead of the Imgur page describing it.  I had to settle with a poor user experience so that others viewing my posts could have a better user experience.

No longer.

ImgurWin's advanced link creation makes it easy to create a link that goes directly to the full size image, linking from any available size of thumbnail.  No hand-editing of the generated markup is required.

Hold the left mouse button on the preview of the image you just uploaded, and this menu will appear.  Release the mouse over the button depicting the item you wish to select.

![Clipboard Menu](http://i.imgur.com/R7nGPKo.png)

# Smart Screen Captures

Similar to the Snipping Tool built in to Windows, ImgurWin allows you to draw a rectangle anywhere on-screen to capture a screenshot of that region.  Once the screenshot is captured, it is automatically compressed, saved to disk, and uploaded to Imgur.

Compression of raw images, including screen captures, is done automatically.  First, ImgurWin compresses the capture to png and jpeg formats (jpeg at 80% quality).  If the png is within 120% of the jpeg's size, then the png version is uploaded because png compression is lossless.  In the rare event that the compressed image is over Imgur's limit of 10 MB, then the image is recompressed at increasingly lower jpeg quality levels until it is smaller than 10 MB.

# Open Source

This project was built because my preferred imgur uploader application was not open source.  So I could not change its behavior when I wanted to.  ImgurWin is open source, so anyone can easily do whatever they like with it.
