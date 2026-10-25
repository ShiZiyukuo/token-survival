# 美术生成记录

工具：内置 ImageGen；未指定模型，未使用名为 image2.5 的型号。最终生成资产全部复制到本工程 Art 目录。图片透明 alpha 已检查。参考视频无法访问，外观依据用户文字说明，动作是原创演绎。

以下为最终项目素材所用的提示词主体。生成时统一附加了真实透明背景（室内背景除外）、无文字、无水印、完整轮廓等生产约束。旧版程序绘制 PNG 没有导入本工程。

## 程序员 player_sheet.png

Consistent chibi programmer character animation sheet for a polished top-down 2D game. One slim young programmer with tousled dark navy hair, round glasses, oversized cream hoodie over teal shirt, dark joggers, soft sneakers, tiny crossbody device pouch. Premium hand-painted anime game sprites, clean silhouette, subtle cel shading, soft outlines, cozy blue-white-coral palette. Exact 4 columns by 4 rows: facing down, left, right, up; each row a seamless 4-frame walk cycle with alternating footfall and natural arm swing; 3/4 top-down orthographic camera. Transparent background, identical face outfit proportions and scale across all frames.

## 大肥鱼状态 whale_sheet.png

Consistent blue-haired chibi whale girl companion animation sheet with clear emotional acting for a polished top-down 2D game. Adorable plump humanoid whale girl, long ocean-blue hair, white frilled seafoam headpiece, blue-and-white sailor dress, whale-tail motif and fin-like sleeves. Premium hand-painted anime sprites, expressive face, refined cel shading, luminous ocean accents. Exact 8 columns by 4 rows: idle/follow, hungry/eating, joyful/dance, bowl raised/strike. Full body and consistent scale, bowl held in front with one hand while other rises and strikes.

## 四方向 whale_walk_sheet.png

Use case: identity-preserve. Asset type: transparent 2D game walking sprite sheet. Reference image is a character identity and style anchor, not layout to preserve. Generate exactly 4 columns by 4 rows, 16 equal spaced cells. Same blue-haired plump chibi humanoid whale girl, white frilled headpiece, blue-white sailor dress, whale fin and tail details; retain face, proportions and outfit. Four true directional walking cycles: row1 front/down, row2 profile facing left, row3 profile facing right, row4 back/up with back of hair and dress visible, no face. Each row four distinct footfalls. Premium anime painted refined cel-shading. Orthographic 3/4 top-down view, feet aligned and same character scale each cell. Generous transparent margins so silhouettes never touch adjacent cells. No text, borders, guide marks, watermark or extra characters. Actual transparent background.

注意：生成结果第二行为向右、第三行为向左，实际 AnimationClip 按检查后的朝向绑定。

## 独立舞蹈 whale_dance_sheet.png

Use case: identity-preserve. Asset type: high-resolution transparent 2D full-body celebration dance sprite sheet, 4 columns by 2 rows, exactly 8 equal cells, large detailed 512px-style character per cell. Use reference only for character identity and painted anime style. Same plump adorable blue-haired humanoid whale girl, white seafoam frill headdress, elaborate blue-white sailor dress with whale-tail motif, fin sleeves. Create a connected 8-frame independent dance animation: 1 cheerful preparation knee bend, 2 stepping right arm flourish, 3 arms overhead turning quarter, 4 full side twirl with hair and skirt following, 5 joyful small leap both feet lifted arms open, 6 landing knee bend, 7 left step and fin-flourish, 8 return celebratory front pose. Real body articulation and dance footwork, varied poses and moving hair and cloth; not static standing sways. Keep identity, scale and outfit identical. Full body never cropped, centered in each equal cell with ample transparent margin. No bowl, props, background, floor shadow, text, borders, watermark. Genuine transparency.

## 室内 room_clean.png

Use case: precise-object-edit. Asset type: polished 16:9 2D game room background. Keep the reference's anime painted art quality, orthographic 3/4 top-down room architecture, warm bed at lower-left, kitchen at right, rain window, blue wood floor, overall framing and lighting. REMOVE all computer desk furniture and showcase furniture across the upper-left and upper-middle, leaving tasteful blank floor space and upper wall decorations so actual separate game furniture sprites can be placed there. REMOVE window-side floor cushion, table, bowl, plant and whale pillows leaving clear floor beneath rainy window. REMOVE plants and cabinets in lower-right intended for a separate food cupboard and upgrade furniture. Preserve floor continuity, no holes or visible empty cutouts. Leave the bed and fixed kitchen sink/counter. No characters, no UI, no labels or text. This must be usable actual game environment art.

## 独立家具（五次独立生成）

共同提示词：Use case: identity-preserve. Asset type: one independent transparent game prop sprite. Reference is STYLE ONLY, generate exactly ONE complete [SUBJECT]. Premium detailed painted anime style, consistent 3/4 top-down orthographic angle, ocean-blue white cream and warm coral palette matching reference. One centered full object group with 15% empty transparent margins on ALL sides, every element entirely inside frame. No neighboring props or fragments, no sprite sheet or grid, no second group, no background scene, no characters, no text or watermark. True transparent background.

- computer.png SUBJECT: compact dual-monitor programmer desk with keyboard, stylish chair and blue whale monitor theme
- showcase.png SUBJECT: small project showcase desk with a luminous miniature whale app holographic diorama
- livestream.png SUBJECT: livestream nook with ring light, microphone, cozy small blue floor cushion and silver donation bowl in front
- food_cabinet.png SUBJECT: small blue-white fish-food cupboard with tins and whale motifs
- upgrade.png SUBJECT: premium upgraded computer tower, plant and warm floor lamp together in a compact home improvement display
