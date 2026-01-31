using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MittelImage : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler
{
    public enum Type{ acid, soap, salt}
    public Type type;
    public Image Img;

    public Sprite defaultSprite;
    public Sprite HoverSprite;
    public Sprite SelectedSprite;
    public Sprite GreySprite;

    public List<MittelImage> Other;
    public bool _enabled = true;

    private void OnEnable()
    {
        Img.sprite = defaultSprite;
        _enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_enabled)
        {
            Img.sprite = defaultSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_enabled)
        {
            Img.sprite = HoverSprite;
            foreach (MittelImage mi in Other)
            {
                mi.Img.sprite = mi.defaultSprite;
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData) 
    {
        if (_enabled)
        {
            _enabled = false;
            Img.sprite = SelectedSprite;
            foreach (MittelImage mi in Other)
            {
                mi.Img.sprite = mi.GreySprite;
                mi._enabled = false;
            }
        }
    }
}
