# LMix

----

基于Leap Motion的3D下落式音乐游戏，使用Unity开发。
A 3D Drop-Music-Game based on Leap Motion developed with Unity.
挥舞双手击打左中右三个方向的note，转动手指旋转转盘，伴着节奏欣赏你喜欢的音乐。
Use your hands hit the note from left, middle and right. Circle your finger to spin spinner. Enjoy music with the rhythm.
较宽的宽判定区避免了误触。
The big judgement area make you won't hit the wrong track.
支持自定义谱面。
You can add your own beatmap.

----

`./Assets/Resources/Default`目录存放默认音效，名为`hit.wav`。
`./Assets/Resources/Default`Store default sound effect, named`hit.wav`.
`./Assets/Resources/Music`目录存放谱面。
`./Assets/Resources/Music`Store beatmaps.
`./Assets/Resources/Music/MusicList.txt`为谱面列表，存放音乐所在文件夹名，每行一个，需自行添加。
`./Assets/Resources/Music/MusicList.txt`Store beatmap list, each line for a folder name. You need add a beatmap to the list by yourselft.
`./Assets/Resources/Music/[Name]`为谱面目录，目录中`beatmap.txt`储存所有谱面信息。
`./Assets/Resources/Music/[Name]`is the beatmap folder, `beatmap.txt`contains all information about the beatmap.
`beatmap.txt`中定义了音频文件名称、封面名称、自定义音效名称、自定义背景视频名称。其中音频文件和封面必须添加。
`beatmap.txt`defines the audio file name, album file name, custom sound effect name, custom background video name. The audio file and album is required.

----

LMix使用了：
LMix uses:
- OpenSans开源字体
- OpenSans open source font
- 从osu!部分谱面中提取的节奏点时间
- Timing point from some osu! beatmaps
- 来源于网络的音乐
- Music from web
- 来源于网络的图片
- Pictures from web
如有侵权请告知我们，将立即删除并进行道歉和赔偿。
If copyright infringement, please notify us. We will remove those file immediately and make apology with compensation.

----

osu!，Cytus，Deemo，Dynamix，感谢这些游戏给我们的指引和灵感。
Thanks for the guide and inspiration from osu!, Cytus, Deemo, Dynamix.