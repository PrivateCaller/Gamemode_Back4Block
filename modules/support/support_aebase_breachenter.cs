package L4B_AEBase_BreachEnter
{
    function BNE_ScarHImage::AEOnFire(%this,%obj,%slot)
    {	
    	%obj.stopAudio(0); 
      %obj.playAudio(0, BNE_ScarHFire @ getRandom(1, 3) @ Sound);
    
    	%obj.blockImageDismount = true;
    	%obj.schedule(200, unBlockImageDismount);
    
    	Parent::AEOnFire(%this, %obj, %slot);
    }    
    
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
};
if(isPackage("L4B_AEBase_BreachEnter")) deactivatePackage("L4B_AEBase_BreachEnter");
activatePackage("L4B_AEBase_BreachEnter");