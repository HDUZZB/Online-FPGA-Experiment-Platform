基于FPGA的在线实验平台
===========================
该项目针对高校硬件课程实验教学的特点和存在的问题，提出了一种基于FPGA的三层构 架的通用实验平台。该平台支持开放性和创新型实验，可应用于数字逻辑电路设计、计算机组成原 理和计算机系统结构等硬件课程的实验教学，实现了多门硬件课程对实验平台的共享。

****
	
|Author|HDUZZB|
|---|---
|E-mail|hduzzb@126.com


****
## 目录
* [系统功能](#系统功能)
* [项目原理](#项目原理)
* [项目技术点](#项目技术点)
* [设计思路](#设计思路)
    * [硬件平台](#硬件平台)
    * [软件平台](#软件平台)
* [测试](#测试)
 
  
系统功能
-----------
项目为基于FPGA开发板的远程实验平台，通过该平台学生只需要在客户端进行操作，便可通过服务器端进行在线远程实验。  
该平台分为硬件平台和软件平台两部分：硬件平台由FPGA开发板组成，用于激励捕获FPGA系统的输入输出信号；软件平台负责实现远程用户的程序上传和FPGA程序运行结果的反馈。  

项目原理
-----------
### 模型及分析
本系统设计是基于TCP的。TCP是面向连接的可靠的传输协议，利用TCP协议进行通信，首先要经过三步握手，以建立通信双方的连接。一旦连接建立好，就可以进行通信。TCP提供了数据的确认和数据的重传机制，保证了发送的数据一定能够到达通信的对方。

### TCP协议
TCP(Transmission Control Protocol, 传输控制协议)协议是一种面向连接的、可靠的协议,它将源主机发出的字节流无差错地发送给互联网上的目标主机。在发送端,TCP协议负责把上层传送下来的数据分成报文段传递给下层;在接收端,TCP协议负责把收到的报文进行重组后递交给上层。开发基于TCP协议的网络程序时使用C/S(客户/服务器)模型，在客户端和服务器端建立连接，使两台主机上的程序顺利通信,不必担心包丢失或包顺序混乱。

### C/S模型流程。
在C/S模型的网络中，服务器是网络的核心，客户机是网络的基础，客户机依靠服务器获得所需要的网络资源，服务器为客户机提供网络所必须的资源。

### SOCKET
所谓Socket通常也称作“套接字”，是网络编程中的一个接口，用于描述IP地址和端口，是一个通信链的句柄，应用程序通过“套接字”向网络发出请求或者应答网络请求。  
在C/S通信模式中，服务器端程序使用ServerSocket类创建监听特定端口的对象，负责接收客户连接请求;客户端程序使用Socket类主动创建与服务器连接的Socket套接字,服务器端收到了客户端的连接请求后,也会创建与客户连接的Socket,此时Socket就是连接两端的收发器,服务器端和客户端通过它来收发数据

项目技术点
-----------
项目技术点主要在于如何使用C#进行网络编程。在本项目中主要`使用C#套接字
`的设计TCP网络应用程序。`实现C#套接字TCP网络通信`涉及到`System.Net.Sockets命名空间`的`Socket类`；S`ystem.IO命名空间`的`StreamReader类`和`StreamWriter类`；`System.Net命名空间`的`IPAdress类`和`IPEndPoint类`。利用这些类可以设计标准的网络应用程序。

### IPAdress类和IPEndPoint类 
System.Net命名空间定义了两个类：`IPAdress`类和`IPEndPoint`类以处理各种类型的`IP地址`信息。  
(1) `IPAddress`类：`IPAddress`对象用于表示一个IP地址。这个值可以在各种不同的套接字中使用。下面的语句用于创建IPAddress 实例：
```cpp 
IPAddress myip=new IPAddress(long address);//默认的构造函数取一个长值，并把它换成一个IPAddress值。 
IPAddress myip=IPAddress.Parse("192.168.1.1");//使用Parse()方法创建一个IPAddress 值。 
```
(2) `IPEndPoint`类：IPEndPoint对象用来表示指定的`IP地址/端口组合`。当把套接字绑定到本地地址的时候，或者在把套接字连接到远程地址的时候，使用`IPEndPoint`对象。下面的语句用于创建IPEndPoint实例： 
```cpp
IPEndPoint myie=new IPEndPoint(long address,int port); 
IPEndPoint myie=new IPEndPoint(IPAddress address,int port); 
```
两个构造函数都使用两个参数：一个是长值表示或者用IPAdress对象表示；另一个是整数端口号。 

### Socket类
在System.Net.Sockets命名空间中则包括Socket类
Socket类:它提供了Winsock API的C#管理代码的实现。下面的语句用于创建Socket实例： 
```cpp
Socket mysocket=new Socket(AddressFamily af,SocketType st,ProtocolType pt); 
```
可以看出,Socket构造函数有三个参数定义创建的套接字类型。用AddressFamily定义网络类型；SocketType定义数据连接的类型；ProtocolType指定具体的网络协议。如创建Tcp网络通信套接字的实例为： 
```cpp
Socket mysocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp); 
```

### StreamReader类和StreamWriter类 
在`System.IO命名空间`的`StreamReader类`和`StreamWriter类`   
(1) `StreamReader类`为许多应用提供多个构造函数，使用
`NetworkStream`是最简单的格式。如下所示： 
```cpp
public StreamReader(Stream stream);//变量stream可以使用任意类型的Stream对象，包括NetworkStream对象。 
``` 
StreamReader类提供了许多属性和方法，其主要的方法是： 
```cpp
Read();//从StreamReader中读取一个或者多个字节的数据。
Readline();//从StreamReader中读取数据，直到第一个换行符(包括该换行符)为止。 
```
(2) StreamWriter类为许多应用提供多个构造函数，使用NetworkStream是最简单的格式。如下所示：
```cpp
public StreamWriter(Stream stream);//变量stream可以使用任意类型的Stream对象，包括NetworkStream对象。
```
StreamWriter类提供了许多属性和方法，其主要的方法是： 
```cpp
Writer();//向下层数据流发送一个或多个字节的数据。 
Writerline();//向下层数据流发送指定的数据和一个换行符。 
Flush();//将所有的StreamWriter缓冲区数据发送到下层数据流中 
```

设计思路
-----------
实验平台以PC机和FPGA教育开发板为基础，采用三层次结构，总体框架如下图所示。  
![实验平台总体架构](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/1.png)  
下层是基于FPGA的学生实验模块，该模块提供了一系列的通用端口，实验者用硬件描述语言完成实验设计，通过通用端口以实现不同硬件课程对该部分的共享。上层为PC机实验软件，该软件以导入或配置实验原理图和动态添加、配置观察信号相结合的方式达到与下层实验内容的一致性，以图形化的界面直观的完成实验过程的控制；中间层是嵌入式实验控制器，为上下层之间的数据通信提供服务。平台利用FPGA的可编程性和大容量，将嵌入式实验控制器和实验模块集成在一块FPGA内部；通过USB接口完成PC机和实验控制器之间的数据传输。  

而TCP网络通信可以由几种方法实现，其核心都是基于Socket进行通信的，所不同的是使用类中的方法不同，传递的信息类型不同。  
`(1)用Socket类中的Send()和Receive()方法发送和接收数据`   
用Socket类中的`Send()`和`Receive()`方法发送和接收数据只能`处理字符型的数组数据`。因此要用`System.Text命名空间`的`Encoding.ASCII类`中的方法`GetByte()`、`GetString()`进行字符串和字符型数据之间相互转换。在TCP网络编程中，如果使用`Socket类`中的`Send()`和`Receive()`方法发送和接收数据，必须考虑`TCP数据缓冲区的处理`和`网络上的TCP消息处理`，否则可能会出现通信中的故障。对于数据缓冲区来说，如果使用的缓冲区太小会导致消息的不匹配；如果使用的缓冲区太大会导致消息的混乱。对于网络上的TCP消息处理，必须考虑消息的边界问题。在网络编程中可以采用几点措施进行解决：`永远发送固定长度的消息`；`将消息尺寸与消息一起发送`；`使用标记系统分隔消息`。  
`(2)用NetworkStream类中的Write()和Read()方法发送和接收数据`  
与Socket类的方法相比，虽然NetworkStream类提供了一些另外的功能，但从套接字中发送和接收数据时`仍然受到限制，同样存在消息边界问题`。  
`(3)用StreamReader类和StreamWriter类`  
StreamReader类和StreamWriter类可以`控制在一个数据流上对文本消息的读写操作`。这样可以较好解决TCP数据缓冲区和TCP消息边界问题。以提高网络通信的质量。  

### 硬件平台
硬件部分我们采用FPGA控制板来进行设计，通过我们相关实验的特点，根据实验中常用到的功能将硬件系统分为供电模块、时钟模块、实验FPGA模块、主控FPGA模块、数据流存储模块、通信模块、激励增加模块和响应收集模块。详细结构如下图所示。  
![硬件系统框图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/2.png)  

### 软件平台
软件平台主要完成实验数据的输入和实验结果的输出。在本设计中通过对FPGA8位流水灯的实验作为演示，设计了相应配套的客户端和服务器端。  
![客户端图片](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/3.jpg)  
![服务器端图片](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/4.jpg)  

首先，我们通过FPGA仿真软件得到相应的数据文件，并进行网络通信将文件传输到服务器端进行远程实验。在使用TCP协议进行通信时，一般服务端进程先使用socket调用得到一个描述符，然后使用bind调用将一个名字与socket描述符连接起来，对于Internet域就是将Internet地址联编到socket之后，服务端使用listen调用指出等待服务请求队列的长度。然后就可以使用accept调用等待客户端发起连接。一般是阻塞等待连接，一旦有客户端发出连接，accept返回客户的地址信息，并返回一个新的socket描述符，该描述符与原先的socket有相同的特性，这时服务端就可以使用这个新的socket进行读写操作了。一般服务端可能在accept返回后创建一个新的进程进行与客户的通信，父进程则再到accept调用处等待另一个连接。客户端进程一般先使用socket调用得到一个socket描述符，然后使用connect向指定的服务器上的指定端口发起连接，一旦连接成功返回，就说明已经建立了与服务器的连接，这时就可以通过socket描述符进行读写操作了。其服务器通信过程与客户机通信过程如下图所示。  
![服务器通信过程](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/5.jpg)  
![客户机通信过程](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/6.jpg)  

测试
-----------
在本设计中通过对FPGA 8位流水灯的实验为例测试实验平台的功能。测试时先通过仿真软件完成硬件设计并保存数据文件到本地，启动客户端并连接到服务器端，再将文件上传到服务器端，再使服务器端运行文件验证实验结果，并将实验结果传输会客户端并在可视化界面中显示。  

### 测试流程

#### 1、首先测试服务器端端口是否开启  
![测试服务器端端口是否开启示意图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/7.png)   
弹出提示信息，端口正常开启  

#### 2、测试客户端连接服务器端是否成功
![测试客户端连接服务器端是否成功示意图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/8.png)   
弹出提示信息，端口连接成功  

#### 3、测试文件传输功能
![测试文件传输功能示意图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/9.png)   
![测试文件传输功能示意图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/10.png)   
文件传输结束后打印提示信息，文件传输及保存成功  

#### 4、测试客户端接受消息开关
![测试客户端连接服务器端是否成功示意图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/11.png)
接收消息开关开启，打印提示信息  

#### 5、测试服务器端能否自动下载程序至FPGA开发板
![测试服务器端能否自动下载程序至FPGA开发板示意图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/12.png)
![测试服务器端能否自动下载程序至FPGA开发板示意图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/13.png)

#### 6、测试服务器端实验显示情况
![测试服务器端实验显示情况示意图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/14.png)

#### 7、测试客户端实验显示情况
![测试客户端实验显示情况示意图](https://github.com/HDUZZB/Online-FPGA-Experiment-Platform/blob/master/image/15.png)
