using System;
using UnityEngine;
namespace TokenSurvival {
    [Serializable] public class DebugContract {
        public string title;
        [TextArea(3,8)] public string code;
        public string failedTest;
        public string[] patches;
        public int correctPatch;
    }
    [CreateAssetMenu(menuName="Token Survival/Contract Catalog")]
    public sealed class ContractCatalog : ScriptableObject {
        public DebugContract[] contracts = {
            new DebugContract {title="购物车边界 Bug",code="int Total(int[] prices) {\n  int sum = 0;\n  for (int i=0; i<=prices.Length; i++)\n    sum += prices[i];\n  return sum;\n}",failedTest="Total([10, 20]) → IndexOutOfRange；期望 30",patches=new[]{"i < prices.Length","i < prices.Length - 1","i >= prices.Length"},correctPatch=0},
            new DebugContract {title="优惠券金额 Bug",code="int Pay(int price, int discount) {\n  return price + discount;\n}",failedTest="Pay(100, 20) → 120；期望 80，金额不能为负",patches=new[]{"price - discount * 2","Math.Max(0, price - discount)","Math.Min(0, price - discount)"},correctPatch=1},
            new DebugContract {title="需求变更 · 免费配送",code="bool FreeShipping(int total) {\n  return total > 50;\n}",failedTest="新需求：满 50 包邮；FreeShipping(50) 期望 true",patches=new[]{"total >= 50","total < 50","total == 50"},correctPatch=0},
            new DebugContract {title="网页空数据处理",code="string Title(string name) {\n  return name.Trim();\n}",failedTest="Title(null) 抛异常；期望未命名项目，其他输入仍去空格",patches=new[]{"name.ToUpper()","name == null ? null : name","string.IsNullOrWhiteSpace(name) ? \"未命名项目\" : name.Trim()"},correctPatch=2},
            new DebugContract {title="分页接口漏数据",code="int Pages(int count, int size) {\n  return count / size;\n}",failedTest="Pages(11, 5) → 2；期望 3，最后一页也要显示",patches=new[]{"count / size + 1","(count + size - 1) / size","count % size"},correctPatch=1}
        };
    }
}
