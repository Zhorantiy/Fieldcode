using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Field : MonoBehaviour {
    public Game game;

    public Database data;

    public inventory invent;

    public List<ItemInnventory> items = new List<ItemInnventory>();

    public GameObject gameObjectShow;
    public int CurrentIDfield;

    public GameObject InventoryMainObject;
    public int MaxCount = 25;

    public Camera cam;
    public EventSystem es;

    public ItemInnventory currentItem;

    public RectTransform movingObject;
    public Vector3 offset;

    public void Start() { 
        AddGraphics();

        for (int i = 0; i < MaxCount; i++) { // Test 
            AddItem(i, data.items[Random.Range(0, data.items.Count)]);
        }
        UpdateInventory();
    }

    public void Update() {
        CurrentIDfield = invent.currentID;
        
        if (invent.currentID != -1) {
            MoveObject();
        }
    }

    public void MakeField() { // создание игрового поля
        for (int i = 0; i < MaxCount; i++) {
            items[i].id = 0;
            items[i].itemGameobj.GetComponent<Image>().sprite = data.items[0].image;
        }

        items[2].id = 6;
        items[2].itemGameobj.GetComponent<Image>().sprite = data.items[6].image; // третья клетка монстр

        for (int i = 5; i < MaxCount; i++) {
            int x = Random.Range(0, 101);
            if (x > 0 && x < 61) {
                continue;
            }
            if (x > 60 && x < 86) { // присвоить армор
                items[i].id = 7;
                items[i].itemGameobj.GetComponent<Image>().sprite = data.items[7].image;
            } else {
                int num = Random.Range(1, 4);
                items[i].id = num;
                items[i].itemGameobj.GetComponent<Image>().sprite = data.items[num].image;
            }
        }
    }

    public void AddItem(int id, Item item) {
        items[id].id = item.id;
        items[id].itemGameobj.GetComponent<Image>().sprite = item.image;
    }

    public void AddInventoryItem(int id, ItemInnventory invItem) {
        items[id].id = invItem.id;
        items[id].itemGameobj.GetComponent<Image>().sprite = data.items[invItem.id].image;
    }

    public void AddGraphics() {
        for (int i = 0; i < MaxCount; i++) {
            GameObject newItem = Instantiate(gameObjectShow, InventoryMainObject.transform) as GameObject;

            newItem.name = i.ToString();

            ItemInnventory ii = new ItemInnventory();
            ii.itemGameobj = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newItem.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);

            Button tempButton = newItem.GetComponent<Button>();

            tempButton.onClick.AddListener(delegate { SelectObject(); });

            items.Add(ii);
        }
    }

    public void UpdateInventory() {
        for (int i = 0; i < MaxCount; i++) {
            items[i].itemGameobj.GetComponent<Image>().sprite = data.items[items[i].id].image;
        }
    }

    public void SelectObject() {
        if (invent.currentID == -1) {

            //если в руке ничего нету
            invent.currentID = int.Parse(es.currentSelectedGameObject.name);

            if (items[invent.currentID].id == 0) {
                invent.currentID = -1;
                return;
            }

            currentItem = CopyInventoryItem(items[invent.currentID]); // копирую элемент
            movingObject.gameObject.SetActive(true); // включаю видимость движущегося предмета
            movingObject.GetComponent<Image>().sprite = data.items[currentItem.id].image; // придаю предмету картинку

            AddItem(invent.currentID, data.items[0]); // добавляю в поле пустой элемент
        } else {
            // если в руке что-то есть
            AddInventoryItem(invent.currentID, currentItem); // добавляю на id = curId а на item = curItem
            invent.currentID = -1; // в руке ничего нет
            game.LetSayAdvisor("Ты пытаешься поменять поле!"); // ошибка!! текст советнику
            movingObject.gameObject.SetActive(false); // выключаю движ предмет
        }
    }

    public void MoveObject() {
        Vector3 pos = Input.mousePosition + offset;
        pos.z = InventoryMainObject.GetComponent<RectTransform>().position.z;
        movingObject.position = cam.ScreenToWorldPoint(pos);
    }

    public ItemInnventory CopyInventoryItem(ItemInnventory old) {
        ItemInnventory New = new ItemInnventory();

        New.id = old.id;
        New.itemGameobj = old.itemGameobj;

        return New;
    }
}
