package L4B_AEBase_BreachEnter
{
    //Let's add some animations from the survivor to this
    //SCARH
    function BNE_ScarHImage::AEOnFire(%this,%obj,%slot)
    {	    
    	Parent::AEOnFire(%this, %obj, %slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"rifleFire1");                
    }

    //UMP45
    function BNE_UMP45Image::AEOnFire(%this,%obj,%slot)
    {	    
    	Parent::AEOnFire(%this, %obj, %slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"rifleFire1");                
    }

    //P90    
    function BNE_P90Image::AEOnFire(%this,%obj,%slot)
    {	    
    	Parent::AEOnFire(%this, %obj, %slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"rifleFire1");                
    }

    //MP7
    function BNE_MP7Image::AEOnFire(%this,%obj,%slot)
    {	    
    	Parent::AEOnFire(%this, %obj, %slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"rifleFire1");                
    }

    //Glock 17
    function BNE_Glock17Image::AEOnFire(%this,%obj,%slot)
    {	
    	Parent::AEOnFire(%this, %obj, %slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"weaponFire1");        
    }

    function BNE_Glock17Image::onReloadStart(%this,%obj,%slot)
    {        
        if(%obj.getDataBlock().isSurvivor)
        {
            %obj.schedule(300, "aeplayThread", "3", "shiftleft");
            %obj.aeplayThread(2, handgunMagOut);
            %obj.aeplayThread(3, wrench);
            %obj.reload3Schedule = %this.schedule(0,onMagDrop,%obj,%slot);
            %obj.reload4Schedule = schedule(getRandom(150,250),0,serverPlay3D,AEMagPistol @ getRandom(1,3) @ Sound,%obj.getPosition());
        }
        else Parent::onReloadStart(%this,%obj,%slot);
    }

    function BNE_Glock17Image::onReloadMagIn(%this,%obj,%slot)
    {
        if(%obj.getDataBlock().isSurvivor)
        {        
            //%obj.schedule(40, "aeplayThread", "3", "plant");
            %obj.schedule(40, "aeplayThread", "2", "handgunMagIn");
            %obj.schedule(300, "aeplayThread", "3", "shiftleft");
        }
        else Parent::onReloadMagIn(%this,%obj,%slot);
    }
    
    function BNE_Glock17Image::onReload2MagIn(%this,%obj,%slot)
    {
        if(%obj.getDataBlock().isSurvivor)
        {
            //%obj.aeplayThread(2, plant);
            %obj.aeplayThread(3, shiftright);
            %obj.schedule(300, "aeplayThread", "2", "handgunMagIn");
            %obj.schedule(400, "aeplayThread", "3", "shiftleft");
        }
        else Parent::onReload2MagIn(%this,%obj,%slot);
    }

    //Deagle
    function BNE_M9Image::AEOnFire(%this,%obj,%slot)
    {	
        Parent::AEOnFire(%this, %obj, %slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"weaponFire1");
    }      
    
    //Deagle
    function BNE_DeagleImage::AEOnFire(%this,%obj,%slot)
    {	
        Parent::AEOnFire(%this, %obj, %slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"weaponFire1");
    }

    function BNE_DeagleImage::onReloadMagIn(%this,%obj,%slot)
    {
        if(%obj.getDataBlock().isSurvivor)
        {
            %obj.schedule(50, "aeplayThread", "3", "shiftright");
            %obj.schedule(500, "aeplayThread", 2, handgunMagIn);
            %obj.schedule(600, "aeplayThread", "3", "shiftleft");
        }
        else Parent::onReloadMagIn(%this,%obj,%slot);
    }

    function BNE_DeagleImage::onReload2MagIn(%this,%obj,%slot)
    {
        if(%obj.getDataBlock().isSurvivor)
        {
            %obj.schedule(50, "aeplayThread", "3", "shiftright");
            %obj.schedule(50, "aeplayThread", "3", "plant");
            %obj.schedule(750, "aeplayThread", 2, handgunMagIn);
        }
        else Parent::onReload2MagIn(%this,%obj,%slot);
    }

    function BNE_DeagleImage::onReloadStart(%this,%obj,%slot)
    {
        if(%obj.getDataBlock().isSurvivor)
        {
            %obj.schedule(300, aeplayThread, 3, wrench);
            %obj.schedule(100, aeplayThread, 2, handgunMagOut);
            %obj.reload3Schedule = %this.schedule(300,onMagDrop,%obj,%slot);
            %obj.reload4Schedule = schedule(getRandom(450,550),0,serverPlay3D,AEMagPistol @ getRandom(1,3) @ Sound,%obj.getPosition());
        }
        else Parent::onReloadStart(%this,%obj,%slot);
    }

    //TT33
    function BNE_TT33Image::AEOnFire(%this,%obj,%slot)
    {	
        Parent::AEOnFire(%this, %obj, %slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"weaponFire1");
    }

    function BNE_TT33Image::onReloadMagIn(%this,%obj,%slot)
    {
        if(%obj.getDataBlock().isSurvivor)
        {
            %obj.schedule(50, "aeplayThread", "3", "shiftright");
            %obj.schedule(500, "aeplayThread", 2, handgunMagIn);
            %obj.schedule(600, "aeplayThread", "3", "shiftleft");
        }
        else Parent::onReloadMagIn(%this,%obj,%slot);
    }

    function BNE_TT33Image::onReload2MagIn(%this,%obj,%slot)
    {
        if(%obj.getDataBlock().isSurvivor)
        {
            %obj.schedule(100, "aeplayThread", "3", "shiftright");
            %obj.schedule(425, "aeplayThread", "3", "shiftleft");
            %obj.schedule(750, "aeplayThread", 2, handgunMagIn);
        }
        else Parent::onReload2MagIn(%this,%obj,%slot);
    }

    function BNE_TT33Image::onReloadStart(%this,%obj,%slot)
    {
        if(%obj.getDataBlock().isSurvivor)
        {
            %obj.schedule(50, aeplayThread, 3, wrench);
            %obj.schedule(100, aeplayThread, 2, handgunMagOut);
            %obj.reload3Schedule = %this.schedule(300,onMagDrop,%obj,%slot);
            %obj.reload4Schedule = schedule(getRandom(450,550),0,serverPlay3D,AEMagPistol @ getRandom(1,3) @ Sound,%obj.getPosition());
        }
        else Parent::onReloadStart(%this,%obj,%slot);
    }

    //Mossberg
    function BNE_M500Image::AEOnLowClimb(%this, %obj, %slot) 
    {
    	Parent::AEOnLowClimb(%this, %obj, %slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"shotgunFire1");
    }

    function BNE_M500Image::onPump(%this,%obj,%slot)
    {
    	Parent::onPump(%this,%obj,%slot);

        if(%obj.getDataBlock().isSurvivor) %obj.playthread(2,"shotgunPump1");
    }    

};

//Eval edits
eval("BNE_870XPItem.AEAmmo = 6;");
eval("BNE_M500Image.spreadBase = 1000;");
eval("BNE_M500Image.spreadMin = 1000;");
eval("BNE_M500Image.spreadMax = 1000;");
eval("BNE_M500Image.projectileDamage = 17;");
eval("BNE_M500Image.projectileCount = 14;");
eval("BNE_870XPImage.spreadBase = 1250;");
eval("BNE_870XPImage.spreadMin = 1250;");
eval("BNE_870XPImage.spreadMax = 1250;");
eval("BNE_870XPImage.projectileDamage = 13;");
eval("BNE_870XPImage.projectileCount = 11;");


if(isPackage("L4B_AEBase_BreachEnter")) deactivatePackage("L4B_AEBase_BreachEnter");
activatePackage("L4B_AEBase_BreachEnter");
