# Token Survival / 养一只大肥鱼

适用编辑器：团结 1.6.12（2022.3.61t13）。旧工程未修改。

入口：`Assets/TokenSurvival/Scenes/MainRoom.unity`。场景、预制体、Animator、AnimationClip、SpriteAtlas 和配置均为保存好的编辑器资产。运行时不生成房间或 UI。

## 操作

- WASD / 方向键：走动。
- 靠近电脑、展示台、直播角、食物柜和设备角后按 E。
- 电脑：阅读代码与失败测试，按 1 / 2 / 3 选择补丁，再按 Enter 运行测试。五道交付题，正确数影响收益。
- 展示台：根据社区关注的功能按提示的 WASD 键回应。及时回应提高赞助。
- 直播角：大肥鱼走向演出位置；光标进入金色拍点区时按 Space，举手拍盆与音效同步。
- 食物柜：花 25 Token 喂食，顺序为吃饭→开心→独立八帧舞蹈→恢复待机或跟随。
- 设备角：购买三档环境升级，提高后续收益。
- Esc：取消当前活动；取消不会产生收益。

无固定六分钟目标，不强制赚 1000 或喂三次。Token、饱腹、完成项目和升级进度自动保存；验证模式不改动玩家存档。

## 编辑

- `Assets/Scripts/`：移动、互动检测、经济、存档、任务、小游戏、角色表现和 UI 分开管理。
- `Assets/TokenSurvival/Config/`：GameConfig 调数值；Contracts 编辑失败测试与补丁选项。
- `Assets/TokenSurvival/Prefabs/`：玩家、鲸鱼娘及五类交互家具；场景引用可在 Inspector 编辑。
- `Assets/TokenSurvival/Animations/`：角色帧动画与控制器；舞蹈使用独立图片与独立 clip。
- `Assets/TokenSurvival/Art/`：统一风格 ImageGen 美术与图集。
- `Assets/TokenSurvival/Audio/`：五类合成 WAV 音效。
- `Assets/Editor/`：一次性生成和验证工具；菜单重新生成会覆盖生成资产，手工修改前先备份。

## 验证边界

`Logs/` 保存真实团结导入、编译、Play Mode 和构建日志；`Validation/` 保存自动化运行报告及运行中游戏相机渲染图片。相机读回图片来自实际 Play Mode/玩家程序；不是素材拼贴，也不是桌面窗口截屏。

美术使用内置 ImageGen，接口不能指定名为 image2.5 的型号。三个 b23.tv 视频未成功访问，角色与动作依据用户文字说明及原创演绎；没有声称逐帧复现参考视频。音效为合成音，目前没有配音或完整背景音乐。
