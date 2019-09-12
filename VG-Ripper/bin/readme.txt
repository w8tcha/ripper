VG-Ripper and VG-Ripper X
------------------------

Features

ViperGirls Ripper is an image ripping tool that downloads all images in a thread or post onto your computer. 
Most major imagehosts are supported. 

- Creates Download Folder based on Subforum-, Thread- and PostTitle
- Auto Rename of Image filename if same filename exists
- Can Save PostIDs to prevent already ripped post
- Presses automaticly the "Thank You" Button on ripped post
- You can specify the number of images needed for the button to use the "Thank You" Button function
- Ripper is multilingual (English, German, French)
- You can Pause Downloads
- When you close the Ripper during ripping and start the programm again you can resume ripping

------------------------

System requirements

- Microsoft .NET Framework 4.7.2 (for Windows)

- Mono Framework (for Linux, Mac OS and other)
  http://www.mono-project.com/Downloads

------------------------

Using VG-Ripper

1) Copy the Adress of the Thread or Post you like to Rip from your Browser to the Windows Clipboard. 
2) RiP-Ripper will automatically start Downloading.

------------------------

Using VG-Ripper X

Use URL:

1) Copy the Adress of the Thread or Post you like to Rip from your Browser to the Windows Clipboard.
2) From the drop-down menu in the top left corner of ripper, select 'URL'.
3) Paste or type in the URL to rip in the edit box to the right side of the drop-down menu.
4) Click 'Start Downloading' and Ripper will start Downloading.

Use Thread ID:

1) From the drop-down menu in the top left corner of ripper, select 'Thread ID'.
2) Paste or type in the Thread ID of the Thread you like to Rip in the edit box to the right side of the drop-down menu.
3) Click 'Start Downloading' and Ripper will start Downloading.

Use Post ID:

1) From the drop-down menu in the top left corner of ripper, select 'Post ID'.
2) Paste or type in the Post ID of the Post you like to Rip in the edit box to the right side of the drop-down menu.
3) Click 'Start Downloading' and Ripper will start Downloading.

------------------------

Hidden Options:

- Offline Modus

If the Forum is offline and you still have some jobs Saved you can run the Ripper in a Offline Modus, just add the following line to your config file (VG-Ripper.exe.config)

<add key="OfflineModus" value="true" />

- FireFox Extension

If you are still using the FireFox Extension for the VG-Ripper you have to Enable it, just add the following line to your config file (VG-Ripper.exe.config)

<add key="FFExtension" value="true" />

- TxtFolder for ExtractUrls.txt folder

If you manually define the location for the folder where the ExtractUrls.txt is located just add the following line to your config file (RiPRipper.exe.config)

<add key="TxtFolder" value="C:\MyFolder\" />