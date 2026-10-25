#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace TokenSurvival.EditorTools {
    public static class ExpandedContractsBuilder {
        public static void Apply(ContractCatalog c){
            c.contracts=new[]{
                Q("C · 数组边界","int a[3]={1,2,3}; return a[3];","下标越界，最后一个元素下标是 2",new[]{"return a[2];","return a[3];","return a[1];"},0),
                Q("C · 指针解引用","int *p=NULL; printf(\"%d\", *p);","空指针不能解引用",new[]{"if(p) printf(\"%d\",*p);","printf(\"%d\",p);","delete p;"},0),
                Q("C · 字符串常量","char *s=\"Token\"; s[0]='t';","字符串常量通常不可写，不能修改字面量",new[]{"const char *s=\"Token\";","char s[] = \"Token\";","两者都不可以"},1),
                Q("C · 内存释放","int *p=malloc(sizeof(int)); *p=7;","堆内存使用后必须释放",new[]{"free(p);","delete p;","release(p);"},0),
                Q("C++ · vector","std::vector<int> v={1,2}; v.push_back(3);","push_back 会把元素追加到末尾",new[]{"v.push_back(3);","v.add(3);","v.append(3);"},0),
                Q("C++ · RAII","std::unique_ptr<File> f(new File());","离开作用域时 unique_ptr 自动释放资源",new[]{"使用 unique_ptr 管理所有权","永远不释放","手动 free(f.get())"},0),
                Q("C++ · const 引用","void Print(const std::string& s)","const 引用避免拷贝且不会修改实参",new[]{"const std::string&","std::string* 并 delete","std::string s[]"},0),
                Q("C++ · 虚函数","Base* b=new Child(); b->Run();","基类指针调用子类实现需要 virtual",new[]{"virtual void Run();","static void Run();","inline void Run();"},0),
                Q("Unity · Update","让角色每帧读取输入并更新动画", "输入和非物理逐帧逻辑适合放在 Update",new[]{"Update()","FixedUpdate()","OnDestroy()"},0),
                Q("Unity · FixedUpdate","让 Rigidbody2D 移动保持物理稳定", "物理步进逻辑适合 FixedUpdate",new[]{"FixedUpdate()","LateUpdate()","OnGUI()"},0),
                Q("Unity · SerializeField","希望私有配置能在 Inspector 编辑", "SerializeField 可让私有字段显示在 Inspector",new[]{"[SerializeField]","[RuntimeOnly]","[ExposePrivate]"},0),
                Q("Unity · GetComponent","获取同一 GameObject 上的 Rigidbody2D", "GetComponent<Rigidbody2D>() 会查找同物件组件",new[]{"GetComponent<Rigidbody2D>()","Find<Rigidbody2D>()","Load<Rigidbody2D>()"},0),
                Q("Unity · 协程","等待 1 秒后继续执行演出", "协程使用 WaitForSeconds",new[]{"yield return new WaitForSeconds(1f);","Thread.Sleep(1);","await Frame(1);"},0),
                Q("Unity · ScriptableObject","保存可复用的任务和数值配置", "ScriptableObject 适合独立配置资产",new[]{"ScriptableObject","MonoBehaviour 只能挂场景","PlayerPrefs 保存全部配置"},0),
                Q("Unity · 预制体","把角色和 Animator 保存为可复用资产", "Prefab 能保存层级、组件和引用",new[]{"Prefab","Screenshot","Tag"},0),
                Q("Unity · LayerMask","让检测只命中家具碰撞层", "LayerMask 可筛选物理查询层",new[]{"LayerMask","SortingLayer 只控制碰撞","CanvasGroup"},0)
            };EditorUtility.SetDirty(c);
        }
        static DebugContract Q(string title,string code,string failed,string[] patches,int correct){return new DebugContract{title=title,code=code,failedTest=failed,patches=patches,correctPatch=correct};}
    }
}
#endif
