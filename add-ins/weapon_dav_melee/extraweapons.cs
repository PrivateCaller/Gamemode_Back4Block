//Don't edit anything, the function in weapon_melee.cs edits the stuff here

AddDamageType(Machete,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Machete> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Machete> %1',0.2,1);
$DamageType::Machete.rBlood = "1";
datablock ItemData(MacheteItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Machete.dts"; uiName = "Machete"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Machete"; colorShiftColor = "0.5 0.5 0.5 1"; image = "MacheteImage";};
datablock ShapeBaseImageData(MacheteImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Machete.dts"; item = "MacheteItem"; doColorShift = MacheteItem.doColorShift; colorShiftColor = MacheteItem.colorShiftColor; meleeDamageDivisor = 1; damageType = $DamageType::Machete; meleeHitEnvSound = "Machete"; meleeHitPlSound = "Machete"; stateTimeoutValue[6] = 0.275;};
function MacheteImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function MacheteImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function MacheteImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(cKnife,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_cKnife> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_cKnife> %1',0.2,1);
$DamageType::cKnife.rBlood = "1";
datablock ItemData(cKnifeItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_cKnife.dts"; uiName = "cKnife"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_cKnife"; colorShiftColor = "0.75 0.75 0.75 1"; image = "cKnifeImage";};
datablock ShapeBaseImageData(cKnifeImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_cKnife.dts"; item = "cKnifeItem"; doColorShift = cKnifeItem.doColorShift; colorShiftColor = cKnifeItem.colorShiftColor; meleeDamageDivisor = 1; damageType = $DamageType::cKnife; meleeHitEnvSound = "Machete"; meleeHitPlSound = "cKnife"; stateTimeoutValue[6] = 0.125;};
function cKnifeImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function cKnifeImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function cKnifeImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(Hatchet,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Hatchet> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Hatchet> %1',0.2,1);
$DamageType::Hatchet.rBlood = "1";
datablock ItemData(HatchetItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Hatchet.dts"; uiName = "Hatchet"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Hatchet"; colorShiftColor = "0.75 0.75 0.75 1"; image = "HatchetImage";};
datablock ShapeBaseImageData(HatchetImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Hatchet.dts"; item = "HatchetItem"; doColorShift = HatchetItem.doColorShift; colorShiftColor = HatchetItem.colorShiftColor; meleeDamageDivisor = 1; damageType = $DamageType::Hatchet; meleeHitEnvSound = "Crowbar"; meleeHitPlSound = "Machete"; stateTimeoutValue[6] = 0.35;};
function HatchetImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function HatchetImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function HatchetImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(Axe,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Axe> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Axe> %1',0.2,1);
$DamageType::Axe.rBlood = "1";
datablock ItemData(AxeItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Axe.dts"; uiName = "Axe"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Axe"; colorShiftColor = "0.75 0.75 0.5 1"; image = "AxeImage";};
datablock ShapeBaseImageData(AxeImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Axe.dts"; item = "AxeItem"; doColorShift = AxeItem.doColorShift; colorShiftColor = AxeItem.colorShiftColor; meleeDamageDivisor = 1; damageType = $DamageType::Axe; meleeHitEnvSound = "Crowbar"; meleeHitPlSound = "Machete"; stateTimeoutValue[6] = 0.4;};
function AxeImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function AxeImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function AxeImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(Spikebat,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Spikebat> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Spikebat> %1',0.2,1);
$DamageType::Spikebat.rBlood = "1";
datablock ItemData(SpikebatItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Spikebat.dts"; uiName = "Spikebat"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Spikebat"; colorShiftColor = "0.675 0.45 0.275 1"; image = "SpikebatImage";};
datablock ShapeBaseImageData(SpikebatImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Spikebat.dts"; item = "SpikebatItem"; doColorShift = SpikebatItem.doColorShift; colorShiftColor = SpikebatItem.colorShiftColor; meleeDamageDivisor = 2; damageType = $DamageType::Spikebat; meleeHitEnvSound = "Bat"; meleeHitPlSound = "Spikebat"; stateTimeoutValue[6] = 0.3;};
function SpikebatImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function SpikebatImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function SpikebatImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(Katana,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Katana> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Katana> %1',0.2,1);
$DamageType::Katana.rBlood = "1";
datablock ItemData(KatanaItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Katana.dts"; uiName = "Katana"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Katana"; colorShiftColor = "0.75 0.75 0.75 1"; image = "KatanaImage";};
datablock ShapeBaseImageData(KatanaImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Katana.dts"; item = "KatanaItem"; doColorShift = KatanaItem.doColorShift; colorShiftColor = KatanaItem.colorShiftColor; meleeDamageDivisor = 2; damageType = $DamageType::Katana; meleeHitEnvSound = "Machete"; meleeHitPlSound = "Machete"; stateTimeoutValue[6] = 0.3;};
function KatanaImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function KatanaImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function KatanaImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(Bat,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Bat> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Bat> %1',0.2,1);
$DamageType::Bat.rBlood = "0";
datablock ItemData(BatItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Bat.dts"; uiName = "Bat"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Bat"; colorShiftColor = "0.675 0.45 0.275 1"; image = "BatImage";};
datablock ShapeBaseImageData(BatImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Bat.dts"; item = "BatItem"; doColorShift = BatItem.doColorShift; colorShiftColor = BatItem.colorShiftColor; meleeDamageDivisor = 3; damageType = $DamageType::Bat; meleeHitEnvSound = "Bat"; meleeHitPlSound = "Bat"; stateTimeoutValue[6] = 0.325;};
function BatImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function BatImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function BatImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(Baton,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Baton> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Baton> %1',0.2,1);
$DamageType::Baton.rBlood = "0";
datablock ItemData(BatonItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Baton.dts"; uiName = "Baton"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Baton"; colorShiftColor = "0.125 0.125 0.125 1"; image = "BatonImage";};
datablock ShapeBaseImageData(BatonImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Baton.dts"; item = "BatonItem"; doColorShift = BatonItem.doColorShift; colorShiftColor = BatonItem.colorShiftColor; meleeDamageDivisor = 2; damageType = $DamageType::Baton; meleeHitEnvSound = "Bat"; meleeHitPlSound = "Bat"; stateTimeoutValue[6] = 0.275;};
function BatonImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function BatonImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function BatonImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(Shovel,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Shovel> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Shovel> %1',0.2,1);
$DamageType::Shovel.rBlood = "0";
datablock ItemData(ShovelItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Shovel.dts"; uiName = "Shovel"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Shovel"; colorShiftColor = "0.5 0.5 0.5 1"; image = "ShovelImage";};
datablock ShapeBaseImageData(ShovelImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Shovel.dts"; item = "ShovelItem"; doColorShift = ShovelItem.doColorShift; colorShiftColor = ShovelItem.colorShiftColor; meleeDamageDivisor = 3; damageType = $DamageType::Shovel; meleeHitEnvSound = "Crowbar"; meleeHitPlSound = "Crowbar"; stateTimeoutValue[6] = 0.45;};
function ShovelImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function ShovelImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function ShovelImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(Sledgehammer,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Sledgehammer> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Sledgehammer> %1',0.2,1);
$DamageType::Sledgehammer.rBlood = "0";
datablock ItemData(SledgehammerItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Sledgehammer.dts"; uiName = "Sledgehammer"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Sledgehammer"; colorShiftColor = "0.8 0.8 0.8 1"; image = "SledgehammerImage";};
datablock ShapeBaseImageData(SledgehammerImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Sledgehammer.dts"; item = "SledgehammerItem"; doColorShift = SledgehammerItem.doColorShift; colorShiftColor = SledgehammerItem.colorShiftColor; meleeDamageDivisor = 2; damageType = $DamageType::Sledgehammer; meleeHitEnvSound = "Crowbar"; meleeHitPlSound = "Sledgehammer"; stateTimeoutValue[6] = 0.4;};
function SledgehammerImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function SledgehammerImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function SledgehammerImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}

AddDamageType(Pan,'<bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Pan> %1','%2 <bitmap:add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/ci_Pan> %1',0.2,1);
$DamageType::Pan.rBlood = "0";
datablock ItemData(PanItem : crowbarItem){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Pan.dts"; uiName = "Pan"; iconName = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/icons/icon_Pan"; colorShiftColor = "0.375 0.375 0.375 1"; image = "PanImage";};
datablock ShapeBaseImageData(PanImage : crowbarImage){shapeFile = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/models/model_Pan.dts"; item = "PanItem"; doColorShift = PanItem.doColorShift; colorShiftColor = PanItem.colorShiftColor; meleeDamageDivisor = 3; damageType = $DamageType::Pan; meleeHitEnvSound = "Pan"; meleeHitPlSound = "Pan"; stateTimeoutValue[6] = 0.225;};
function PanImage::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}
function PanImage::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}
function PanImage::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}
