# LMix

----

## 简介

基于Leap Motion的3D下落式音乐游戏，使用Unity开发。  
挥舞双手击打左中右三个方向的note，转动手指旋转转盘，伴着节奏欣赏你喜欢的音乐。  
较宽的宽判定区避免了误触。  
支持自定义谱面。  
官方页面：http://liu946.github.io/LMixMainPage/  
aLIEz试玩演示：http://www.bilibili.com/video/av2477424/  
游戏宣传及更多演示：http://www.bilibili.com/video/av2476095/  

## Introduction

A 3D Drop-Music-Game based on Leap Motion developed with Unity.  
Use your hands hit the note from left, middle and right. Circle your finger to spin spinner. Enjoy music with the rhythm.  
The big judgement area make you won't hit the wrong track.  
You can add your own beatmap.  
Official Page: http://liu946.github.io/LMixMainPage/  
Demo1：http://www.bilibili.com/video/av2477424/  
Demo2: http://www.bilibili.com/video/av2476095/  

----

## 玩法

选曲界面：左右挥手切换歌曲，向下挥手切换难度，向上挥手选定歌曲。  
游戏界面：尽可能在note达到判定线时击打note。白色note需要击打，灰色note只需接住。手移动到左上方游戏暂停/恢复，移动到右上方返回选曲界面。  
计分界面：向下挥手返回选曲界面，向上挥手重玩本曲。  

**为了让更多人能够体验到，现已加入键盘操作支持！**  
选曲界面：左右方向键切换歌曲，下方向键切换难度，上方向键选择(WASD也可以喔！)。    
游戏界面：DFJK分别用于击打四条轨道，Space暂停，Esc返回选曲界面。    
计分界面：下方向键或Esc返回选曲界面，上方向键或者R重玩本曲。  

## How to play

Select music scene: Swipe your hand right and left to switch. Swipe your hand down to change difficulty. Swipe your hand up to select.   
Game scene: Hit the note when it reach the line. You need to hit white note and catch the grey note. Move your hand to upleft area to pause/resume, upright to exit.   
Scoring scene: Swipe your hand down to back to select music scene. Swipe your hand up to retry.  

**Keyboard control supportted now!**  
Select music scene: ←|→ to switch. ↓ to change difficulty. ↑ to select.   
Game scene: DFJK controls four tracks. Spcae to pause/resume, Esc to exit.   
Scoring scene: ↓ to back to select music scene. ↑ to retry.  

----

## 项目说明

开发环境：Windows 7 Ultimate 64-bit, Unity 5.0.1f1; OS X 10.10, Unity 5.0.1f1  
**谱面中音乐文件没有添加到项目中，请自行下载或去osu! beatmap中提取。**  
正确放置所有音乐后即可正常编译运行。  
`./Assets/Resources/Default`目录存放默认音效，名为`hit.wav`。  
`./Assets/Resources/Music`目录存放谱面。    
`./Assets/Resources/Music/MusicList.txt`为谱面列表，存放音乐所在文件夹名，每行一个，需自行添加。  
`./Assets/Resources/Music/[Name]`为谱面目录，目录中`beatmap.txt`储存所有谱面信息。  
`beatmap.txt`中定义了音频文件名称、封面名称、自定义音效名称、自定义背景视频名称。其中音频文件和封面必须添加。  

## Project Guide

Development environment: Windows 7 Ultimate 64-bit, Unity 5.0.1f1; OS X 10.10, Unity 5.0.1f1    
**The audio file of beatmap is not in repo. Please download by yourself or extract from the osu! beatmap.**   
If you put all audio file properly, you may build and run successfully.   
`./Assets/Resources/Default`Store default sound effect, named`hit.wav`.   
`./Assets/Resources/Music`Store beatmaps.  
`./Assets/Resources/Music/MusicList.txt`Store beatmap list, each line for a folder name. You need add a beatmap to the list by yourselft.  
`./Assets/Resources/Music/[Name]`is the beatmap folder, `beatmap.txt`contains all information about the beatmap.  
`beatmap.txt`defines the audio file name, album file name, custom sound effect name, custom background video name. The audio file and album is required.  

----

## 引用

LMix使用了：  

- OpenSans开源字体
- 从osu!部分谱面中提取的内容
- 来源于网络的音乐
- 来源于网络的音效
- 来源于网络的图片  

所有引用再此列出：  
https://osu.ppy.sh/b/170608 - 谱面  
https://osu.ppy.sh/b/415060 - 谱面  
https://osu.ppy.sh/b/371225 - 谱面  
https://osu.ppy.sh/b/103282 - 谱面  
https://osu.ppy.sh/b/123417 - 谱面  
https://osu.ppy.sh/b/73817 - 谱面  
https://osu.ppy.sh/b/86324 - 谱面  
https://osu.ppy.sh/b/510873 - 谱面  
https://osu.ppy.sh/b/497557 - 谱面  
https://osu.ppy.sh/b/123472 - 谱面  
http://sc.chinaz.com/yinxiao/1403182524.htm - 音效  

如有侵权请告知我们，将立即删除并进行道歉和赔偿。  

## Reference

LMix uses:  
  
- OpenSans open source font   
- Content from some osu! beatmaps  
- Music from web  
- Sound effect from web
- Pictures from web  

Following is what we used:  
https://osu.ppy.sh/b/170608 - beatmap  
https://osu.ppy.sh/b/415060 - beatmap  
https://osu.ppy.sh/b/371225 - beatmap  
https://osu.ppy.sh/b/103282 - beatmap  
https://osu.ppy.sh/b/123417 - beatmap  
https://osu.ppy.sh/b/73817 - beatmap  
https://osu.ppy.sh/b/86324 - beatmap   
https://osu.ppy.sh/b/510873 - beatmap   
https://osu.ppy.sh/b/497557 - beatmap   
https://osu.ppy.sh/b/123472 - beatmap   
http://sc.chinaz.com/yinxiao/1403182524.htm - Sound effect  

If copyright infringement, please notify us. We will remove those file immediately and make apology with compensation.  

----

## 联系我们 | Contact

- 39M: 39M@outlook.com  
- liu946: liuyang570926881@Gmail.com  

----

## 致谢

osu!，Cytus，Deemo，Dynamix，感谢这些游戏给我们的参考。

## Thanks
  
Thanks for the reference from osu!, Cytus, Deemo, Dynamix.  