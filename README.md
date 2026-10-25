# Token Survival / 养一只大肥鱼

本仓库以使用者指定的最终工程版本为唯一来源，不混入早期导入包或旧APK。该来源对应本机TokenSurvivalRebuild工程；本次发布保留源码、场景、预制体、配置、AI辅助美术和合成音效，排除构建包、缓存与玩家存档。

## 运行

使用团结引擎1.6.12 / 2022.3.61t13打开仓库根目录，进入Assets/TokenSurvival/Scenes/MainRoom.unity后播放。其他Unity版本未在本次验证。

WASD/方向键走动，靠近家具按E；电脑做补丁任务，展示台回应按键，直播角敲盆，食物柜喂食，设备角升级，Esc取消活动。详见[实现记录](docs/implementation-notes.md)。编辑器重新生成工具可能覆盖已有资产，修改前先提交当前版本。

## 来源与验证范围

使用者设计与迭代，Codex协助实现和重建；图像含ImageGen辅助生成，音效为合成。角色包含二创元素，未声称拥有第三方角色设计。具体提示与范围见ART_PROMPTS.md和RIGHTS.md。

[历史验收记录](docs/historical-validation.md)记载播放器自动检查；此次整理不等于重新完成真人全流程或手机实机验收。后续可在仓库继续修改，并在Releases单独发布确认过的试玩包。
