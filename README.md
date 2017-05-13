RansomHoneyPot
==============
This is an experimental and incomplete program for detecting if any ransomware is attacking your files. Currently is in alpha stage.

What is Ransomware?
-------------------
[Ransomwares](https://en.wikipedia.org/wiki/Ransomware) are malicious program which will try to encrypt all your files in background, and ask you for money in order to decrypt.

So, how the honey pot works? Or how *should* it works?
------------------------------------------------------
1. This program will start track the "honey pot" files named and located with file extention and path which will likely to be encrypted by ransomwares.
2. Once the file is opened by other program (i.e. file lock is created), this program will immediately kills those process as those should consider ransomwares.

You may give it a try, but currently there is no guarantee that it can be 100% accurate.

Reference
---------
- https://blogs.msdn.microsoft.com/oldnewthing/20120217-00/?p=8283
- https://stackoverflow.com/questions/317071/how-do-i-find-out-which-process-is-locking-a-file-using-net

Contributing
------------
Yes, go on fork one and modify it!

License
-------
[MIT](LICENSE)