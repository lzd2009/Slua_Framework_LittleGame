ʹ��slua��һ���򵥵Ŀ�ܡ�
��ܲ��ֶ���C#�˱�д��������
��Դ�ȸ��£�����ÿ��assetBundle��md5ֵ->��¼��һ��version.txt�ļ���->��assetBundle�Ǻ�version.txt�ϴ������������ͻ�������ʱ�����ȿ���������version.txt�ļ������������version.txt�ļ����бȶԣ��ѱ���û�еĺ�md5ֵ��ͬ��ab���������Ǽ�¼������Ȼ��ӷ��������ص����ء�
lua�ļ��ļ��أ�lua�ļ�ȫ�������ͬһ��assetBundle������lua.ab������Դ�ȸ���ɺ󣬴�lua.ab��ȡ������lua�ļ������ļ�����byte[]�洢���ֵ��doFile��ʱ����ֵ���ȡ��
lua��unity�������ں����Ľ�����д��һ��LuaBehaviour.cs�ű��̳�monoBehaviour��һ��LuaBehaviour.cs��һ��lua table����start,update,fixedUpdate�ȷ����׽ӵ�lua��ִ�С���lua��ʹ�õ�ʱ�򣬴���Ӧ����������
    local characterAsset = AssetManager.LoadAsset('character.ab','character');
    local characterGo = GameObject.Instantiate(characterAsset);
    local Character = AssetManager.Require('character.txt') 
    local character = Character:new(); 
    characterGo:AddComponent(LuaBehaviour):BindLuaTable(character);
��Դ������assetManager����Ҫ��ָ��ab����ȡ��ָ�����ֵ���Դ������ab��������Դ������Դ������������ָ��ab��������������������Ҳ�����أ���ȡ��ָ����Դ��ȡ���󣬽�ֱ��assetBundle.UnLoad(false)�Ѽ��ص�ab������������ж�ص�������һ�����ڴ档���س�����asset����assetManager��ӵ��ֵ��������Ϊ��Դ����ֵΪasset�����´�������������Դ�����ֵ���ֱ��ȡ�����ء��л��ؿ���ʱ����Ҫ�������ֵ䡣��Resources.UnLoadUnusedAssets()�����ܹ�������Щ��Դ��
����أ�д��һ��gameObject�Ķ���ء�
��Ϣ�ɷ���ʹ�ü�����ģʽ����Ϣ����ά����һ���ֵ䣺Dictionary<string, List<LuaTable>> handlerDic��string�����¼�����list���ǹ�������¼�������table��û��ʹ��c#��ί����ʵ�ּ�����ģʽ����Ϊ�������Ļ������¼�����ʱ��lua�˵ķ������ò���self������
����״̬������lua��д�˽�ɫ�͵��˵�����״̬����


