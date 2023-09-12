using System.Collections.Generic;
using UnityEngine;

public class MenuStack
{
    public int Count => menus.Count;

    private Stack<GameObject> menus = new();

    public void OpenMenu(GameObject menu)
    {
        if (menus.Count > 0) SetVisibilty(menus.Peek(), false);
        menus.Push(menu);
        SetVisibilty(menu, true);
    }
    
    public void PreviousMenu()
    {
        if (menus.Count > 0) SetVisibilty(menus.Pop(), false);
        if (menus.Count > 0) SetVisibilty(menus.Peek(), true);
    }
    
    private void SetVisibilty(GameObject menu, bool state)
    {
        menu.SetActive(state);
    }
}