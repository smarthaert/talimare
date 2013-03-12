
function Hide()
{
	allRenderer = GetComponentsInChildren(Renderer);
	for (var rendR : Renderer in allRenderer)
	{
		rendR.enabled = false;
	}
}

function Show()
{
	allRenderer = GetComponentsInChildren(Renderer);
	for (var rendR : Renderer in allRenderer)
	{
		rendR.enabled = true;
	}
}
