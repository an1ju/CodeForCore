# CodeForCore
2020年03月25日09:33:37
为了实验平台跨平台准备

2020年03月26日09:56:43
有进展了！

2020年03月27日14:10:19
今天开始安装MATLAB到ubantu上。记录一下几条linux命令

mkdir ~/matlab        //用户主目录下新建文件夹 matlab
sudo mount -o loop R2018b_glnxa64_dvd1.iso ~/matlab   //将 iso 文件挂载在 ~/matlab 目录下
sudo sudo ~/matlab/install        //执行安装程序

sudo mount -o loop R2018b_glnxa64_dvd2.iso ~/matlab

我下载的MATLAB2018b可能是光盘有问题。

2020年03月30日15:51:01
遇到一个很奇怪的事情：关于 string.replace(a,b);
在windows framework中，双引号被自动屏蔽了。
而在.net Core中，字符串中的双引号是被认知的。


