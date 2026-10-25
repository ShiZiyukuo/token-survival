# 实际验证结果

环境：本机 Windows，团结 1.6.12 / 2022.3.61t13。

## 导入、编译与构建

- 已执行本机编辑器导入和脚本编译，生成并保存 MainRoom 场景、7 个 prefab、20 个 AnimationClip、2 个 AnimatorController、1 个 SpriteAtlas、2 个配置资产及 5 个 WAV。
- Windows 构建：Succeeded，构建错误 0。最终日志为 `Logs/windows-build-final.log`。
- Windows 可执行文件：`Build/Windows/TokenSurvival.exe`，已实际启动并完成下列验证。

## 运行验证

团结 Play Mode：完整操作流程通过，`Logs/playmode5.log` 的结果为 `PLAYMODE_RESULT errors=0`。

最终 Windows 玩家程序：26 项自动化检查通过，失败 0，捕获的运行时 Console 错误/异常 0。详细报告：`Build/Windows/Validation/playmode-report.txt`。最终运行日志：`Logs/windows-player-final.log`。

覆盖内容：配置引用、Rigidbody2D 移动、床体碰撞、所有五个家具的实际走动可达性与靠近互动、代码补丁交付及收益、项目展示及收益、饱腹消耗与饥饿帧动画、喂食→吃饭→开心→独立舞蹈→正常状态、走向直播位置、敲盆小游戏结束、五次动画拍击事件触发五次音效调用，以及 Token 购买可见升级。

验证说明：运行验证通过控制器输入与共享互动/小游戏提交接口驱动真实游戏循环；没有人工键鼠试玩。饥饿测试在运行中调用时间推进接口模拟 600 秒，未真实等待十分钟。验证模式隔离正常存档。

## 图片真实性

`Build/Windows/Validation/01-room.png` 到 `07-upgrade.png` 是最终 Windows 游戏运行中，通过游戏相机渲染并读回的 1600×900 图片。它们不是素材拼接、不是编辑器静态场景预览，也不是桌面窗口截屏。

## 剩余限制

- 三个 b23.tv 参考视频未成功取得，未实际观看；动作来自文字要求及原创演绎，不能声称复现 57～59 秒原视频动作。
- 使用内置 ImageGen，接口无法指定 image2.5 型号；生成提示词记录于 ART_PROMPTS.md。
- 当前是单房间可玩重构版。AI 人格、更多外包内容与长期成长剧情没有加入；音效为合成 WAV，暂没有配音和完整背景音乐。
- 程序员待机使用步态中相近姿势的两帧小幅循环，还没有专门绘制的呼吸/眨眼表；鲸鱼娘有独立待机表、四方向走动表、独立八帧舞蹈和八帧敲盆。
- 三档升级目前以设备角的颜色、尺寸与收益加成表现，尚未做到三套完全不同的装修主题。
- 编辑器启动日志存在授权客户端签名/刷新提示，随后完成授权；最终构建成功，游戏运行期未捕获 Console 错误。

旧工程未被修改，所有新文件均位于 E:\TokenSurvivalRebuild。
