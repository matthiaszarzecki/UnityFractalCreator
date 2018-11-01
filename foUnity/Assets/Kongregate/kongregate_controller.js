#pragma strict
#if UNITY_WEBPLAYER
var kongregate_prefab: GameObject;

static var kongregate_active = false;

function Start()
{
	if(kongregate_controller.kongregate_active)return;
	
	kongregate_active = true;
	Instantiate(kongregate_prefab);
}
#endif