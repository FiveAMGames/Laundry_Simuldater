using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MittelImage : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler
{
    public bool isVorwaesche = true;


    public Logic.Outfit.Vorwaeasche type;
    public Logic.Outfit.Waesche Waaeschetype;
    public Image Img;
    public SpriteRenderer ImgSprite;



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
           if (Img) Img.sprite = defaultSprite;
            if (ImgSprite) ImgSprite.sprite = defaultSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_enabled)
        {
            if (Img) Img.sprite = HoverSprite;
            if (ImgSprite) ImgSprite.sprite = HoverSprite;
            foreach (MittelImage mi in Other)
            {
                if (mi.Img) mi.Img.sprite = mi.defaultSprite;
                else mi.ImgSprite.sprite = mi.defaultSprite;
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData) 
    {
        if (_enabled)
        {
            if (isVorwaesche)
            {
                Logic.Instance.SelectVorwaesche(type);
            }
            else Logic.Instance.ClickWaschMittel(Waaeschetype);
            _enabled = false;
            if (Img) Img.sprite = SelectedSprite;
            if (ImgSprite) ImgSprite.sprite = SelectedSprite;
            foreach (MittelImage mi in Other)
            {
                if (mi.Img) mi.Img.sprite = mi.GreySprite;
                else mi.ImgSprite.sprite = mi.GreySprite;
                mi._enabled = false;
            }
        }
    }
}
