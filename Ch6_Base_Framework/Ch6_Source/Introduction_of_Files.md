# Framework Library File Information

Author: DarKRabbit
Blog: [CSDN](http://blog.csdn.net/DarkRabbit)
Unity Version: 2017.3.0f3

The introduction of files.
文件介绍 

## Singleton Folder

- Singleton.cs
> This script is the base class of singleton class except MonoBehaviour.
> 所有除了组件的单例类的基类

- UnitySingleton.cs
> This script is the base class of singleton MonoBehaviour.
> 所有单例组件的基类

- CoroutineInstance.cs
> This script is a sealed class inherits 'UnitySingleton'. 
> When 'activeInHierarchy' of other GameObject is false, you can use this to 'StartCoroutine'.
> 继承自'UnitySingleton'。
> 当其它游戏物体的'activeInHierarchy'为false时，你可以使用这个脚本来'StartCoroutine'。

## EventCenter Folder

- MessageCenter.cs
> This script is used to register(unregister, send) messages.
> 负责注册（注销，分发）消息。

- IMessageHandler.cs
> This script is the interface that is used to notify the 'MessageCenter' who is a handler.
> 消息处理者接口，在'MessageCenter'中被使用。

- MessageControllerBase.cs
> This script is the base class of controller for handling messages.
> 所有处理消息的Controller的基类。

- MessageArgs.cs
> This script is the base class of 'Message Arguments'.
> 消息参数的基类。

## Application Folder

- ApplicationEntry.cs
> This script is the base MonoBehaviour of 'Application Entry'. 
> Game can only have one Entry.
> The entry is also a extension of the 'UnityEngine.SceneManagement.SceneManager'.
> 游戏入口基类。
> 游戏只能有一个入口。
> 它同时还是'UnityEngine.SceneManagement.SceneManager'的扩展。

- LoadSceneType.cs
> This script is enum. 'Build Index' or 'Scene Name' to load the scene.
> 读取场景的类型。使用'Build Index'或者'Scene Name'。

- OnActiveSceneChangedArgs.cs
> This script inherits from 'MessageArgs'. 
> When the active scene changed, 'ApplicationEntry' will send a message named 'Event_OnActiveSceneChanged'.
> 继承自'MessageArgs'。
> 当激活场景被改变时，'ApplicationEntry'会发送'Event_OnActiveSceneChanged'事件。

- OnLoadSceneArgs.cs
> This script inherits from 'MessageArgs'. 
> Before loading scene, 'ApplicationEntry' will send a message named 'Event_OnLoadScene'.
> 继承自'MessageArgs'。
> 读取场景之前，'ApplicationEntry'会发送'Event_OnLoadScene'事件。

- OnSceneLoadedArgs.cs
> This script inherits from 'MessageArgs'. 
> When scene is loaded, 'ApplicationEntry' will send a message named 'Event_OnSceneLoaded'.
> 继承自'MessageArgs'。
> 场景加载完成时，'ApplicationEntry'会发送'Event_OnSceneLoaded'事件。

- OnSceneLoadedArgs.cs
> This script inherits from 'MessageArgs'. 
> When scene is unloaded, 'ApplicationEntry' will send a message named 'Event_OnSceneUnloaded'.
> 继承自'MessageArgs'。
> 场景卸载完成时，'ApplicationEntry'会发送'Event_OnSceneUnloaded'事件。

## ObjectPools Folder

- PoolManager.cs
> This script inherits from 'Singleton'. It used to manage the pool.
> 继承自'Singleton'，用它来管理对象池。

- ObjectPool.cs
> This script is a sealed MonoBehaviour. It used to spawn(despawn) instance.
> 继承自MonoBehaviour的类，不可再次继承。使用它来生成或回收instance。

- ReusableObject.cs
> This script is a sealed MonoBehaviour.
> When the pool create an new instance, the instance will add it.
> 继承自MonoBehaviour的类，不可再次继承。
> 当对象池创建instance时，新创建的instance会挂载它。

- IReusableComponent.cs
> When the pool spawn(despawn) an instance, Monobehaviour inherits it will trigger 'OnSpawn(OnDespawn)'.
> 当对象池生成（回收）instance时，继承了它的MonoBehaviour会触发'OnSpawn（OnDespawn）'。 

- RuntimePrePoolObject.cs
> This script is a sealed MonoBehaviour. It used to add the unpooled instance to the pool.
> It can only be added to the prefab instance in the scene.
> 继承自MonoBehaviour的类，不可再次继承。使用它将不由对象池生成的对象加入到对象池中。
> 它只能挂载在场景中的预制体对象上。

## Configs Folder

- ConfigLoader.cs
> This script is used to load bytes of config file.
> If you use WWW load config from internel, you need to change the source code to async, and add a timeout param.
> 用于读取Config文件的Bytes。
> 如果你使用WWW从网络读取配置文件，你需要修改源代码，从同步读取更改成异步读取，并添加超时变量。

- ConfigFile.cs
> This script is the base class of config.
> 所有Config类的基类。

- XmlConfigFile.cs
> This script is the base class of xml config loaded from 'XmlSerializer'.
> 所有用Xml序列化的XmlConfig类的基类。

- JsonConfigFile.cs
> This script is the base class of json config.
> 所有JsonConfig类的基类。

- TxtConfigFile.cs
> This script is the base class of txt config.
> 所有TxtConfig类的基类。

- XmlDocConfigFile.cs
> This script is the base class of xml config loaded from 'XmlDocument'.
> 所有用XmlDocument的XmlConfig类的基类。

## Views Folder

- ViewManagerBase.cs
> This script is a base class of 'ViewManager'.
> It is used to manage the views. The core algorithm has been separated into the file 'ViewDictionary.cs'.
> 视图管理器的基类。
> 用于管理视图。核心算法已经被分离到'ViewDictionary.cs'文件中。

- ViewDictionary.cs
> This script is used to manage the views. Separation from the 'ViewManagerBase.cs'. 
> 用于管理视图。从'ViewManagerBase'中分离。

- ViewBase.cs
> This script is the base class of view MonoBehivour.
> 视图组件的基类。

- UIBase.cs
> This script is the base class of ui MonoBehivour.
> UI组件的基类。

- FrameworkUIUtility.cs
> Utility of ui. Just to get the 'EventSystem' in the scene.
> UI工具类，仅仅用于获取场景中的'EventSystem'。

## Tools Folder

- DontDestroyGameObject.cs
> This script is a sealed MonoBehaviour to make the GameObject 'DontDestroyOnLoad'.
> 挂载这个组件的GameObject，将执行DontDestroyOnLoad(gameObject)。
