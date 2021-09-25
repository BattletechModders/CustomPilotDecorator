using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomPilotDecorator {
  public class ShuffleGridLayoutElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private ShuffleGridLayoutGroup m_suffleGroup = null;
    public Image background { get; set; } = null;
    public int index { get; set; }
    public int weight { get; set; }
    public ShuffleGridLayoutGroup suffleGroup {
      get {
        if (m_suffleGroup != null) { return m_suffleGroup; }
        m_suffleGroup = this.transform.parent.gameObject.GetComponent<ShuffleGridLayoutGroup>();
        return m_suffleGroup;
      }
    }
    private RectTransform m_rectTransform = null;
    private static Vector3 m_smallVector = new Vector3(0.8f, 0.8f, 1.0f);
    public RectTransform rectTransform { get { if (m_rectTransform != null) { return m_rectTransform; } m_rectTransform = this.transform as RectTransform; return m_rectTransform; } }
    public void Awake() {
      background = this.gameObject.GetComponent<Image>();
    }
    public void Update() {
      if (background == null) { return; }
      if (background.color.a < 1.0f) {
        Color color = background.color;
        color.a = 1.0f;
        background.color = color;
      }
    }
    public void OnPointerEnter(PointerEventData eventData) {
      if (this.suffleGroup == null) { return; }
      this.transform.SetAsLastSibling();
      this.transform.localScale = Vector3.one;
      this.suffleGroup.currentElement = this;
      int siblingIndex = this.transform.GetSiblingIndex();
      for (int t = this.index + 1; t < suffleGroup.elements.Count; ++t) {
        siblingIndex -= 1;
        suffleGroup.elements[t].transform.SetSiblingIndex(siblingIndex);
      }
      for (int t = this.index - 1; t >= 0; --t) {
        siblingIndex -= 1;
        suffleGroup.elements[t].transform.SetSiblingIndex(siblingIndex);
      }
      //Camera mainUiCamera = LazySingletonBehavior<UIManager>.Instance.UICamera;
      //if (mainUiCamera == null) { return; }
      //Vector3 worldClickPos = mainUiCamera.ScreenToWorldPoint(eventData.position);
      //Vector3[] corners = new Vector3[4];
      //this.rectTransform.GetWorldCorners(corners);
      //Log.TWL(0, "ShuffleGridLayoutElement.OnPointerEnter index:" + this.orderedLayoutElement.index + " localPos:" + localPost);
    }
    public void OnPointerExit(PointerEventData eventData) {
      if (this.suffleGroup == null) { return; }
      if (this.suffleGroup.currentElement == this) { this.suffleGroup.currentElement = null; };
      this.transform.localScale = m_smallVector;
      //Camera mainUiCamera = LazySingletonBehavior<UIManager>.Instance.UICamera;
      //if (mainUiCamera == null) { return; }
      //Vector3 worldClickPos = mainUiCamera.ScreenToWorldPoint(eventData.position);
      //Vector3[] corners = new Vector3[4];
      //this.rectTransform.GetWorldCorners(corners);
    }
  }
  public class ShuffleGridLayoutGroup : MonoBehaviour {
    public OrderedGridLayoutGroup layoutGroup { get; set; }
    public List<ShuffleGridLayoutElement> elements { get; set; } = new List<ShuffleGridLayoutElement>();
    public ShuffleGridLayoutElement currentElement { get; set; } = null;
    private RectTransform m_rectTransform = null;
    public RectTransform rectTransform { get { if (m_rectTransform != null) { return m_rectTransform; } m_rectTransform = this.transform as RectTransform; return m_rectTransform; } }
    private static Vector3 m_smallVector = new Vector3(0.8f, 0.8f, 1.0f);
    public void Awake() {
      this.Refresh();
    }
    public void Refresh() {
      this.layoutGroup = this.gameObject.GetComponent<OrderedGridLayoutGroup>();
      ShuffleGridLayoutElement[] trs = this.gameObject.GetComponentsInChildren<ShuffleGridLayoutElement>(true);
      elements.Clear();
      foreach (ShuffleGridLayoutElement tr in trs) {
        if (tr.transform.parent != this.transform) { continue; }
        elements.Add(tr);
      }
      elements.Sort((x, y) => { return x.weight - y.weight; });
      for (int t = 0; t < elements.Count; ++t) {
        elements[t].index = t;
      }
    }
    public void Update() {
      foreach (ShuffleGridLayoutElement tr in elements) {
        tr.transform.localScale = tr == this.currentElement ? Vector3.one : m_smallVector;
      }
      float view_width = 240.0f;//this.rectTransform.sizeDelta.x - this.layoutGroup.padding.left - this.layoutGroup.padding.right;
      float components_width = 0f;
      int count = 0;
      foreach (ShuffleGridLayoutElement tr in elements) {
        if (tr.gameObject.activeInHierarchy == false) { continue; }
        count += 1;
        components_width += tr.rectTransform.sizeDelta.x;
      }
      float spacing_x = 2f;
      if (count > 1) {
        spacing_x = (view_width - components_width) / ((float)(count - 1));
      }
      //Log.TWL(0, "ShuffleGridLayoutGroup.Update spacing_x:"+ spacing_x + " view_width:" + view_width+ " components_width:" + components_width+ " count:" + count);
      Vector2 spacing = layoutGroup.spacing;
      if (spacing_x >= 2f) {
        spacing.x = 2f;
      } else {
        spacing.x = spacing_x;
      }
      layoutGroup.spacing = spacing;
    }
  }
  public class OrderedGridLayoutGroup : GridLayoutGroup {
    protected virtual List<KeyValuePair<RectTransform, ShuffleGridLayoutElement>> orderChildren { get; set; } = new List<KeyValuePair<RectTransform, ShuffleGridLayoutElement>>();
    public virtual void PopulateChilds() {
      this.rectChildren.Clear();
      this.orderChildren.Clear();
      ShuffleGridLayoutElement[] elements = this.gameObject.GetComponentsInChildren<ShuffleGridLayoutElement>();
      foreach (ShuffleGridLayoutElement el in elements) {
        if (el.transform.parent != this.transform) { continue; }
        RectTransform child = el.transform as RectTransform;
        if (child != null) { this.orderChildren.Add(new KeyValuePair<RectTransform, ShuffleGridLayoutElement>(child, el)); }
      }
      orderChildren.Sort((x, y) => { return x.Value.weight - y.Value.weight; });
      foreach (var el in orderChildren) {
        rectChildren.Add(el.Key);
      }
      this.m_Tracker.Clear();
    }
    public override void CalculateLayoutInputHorizontal() {
      this.PopulateChilds();
      int constraintCount;
      int num;
      if (this.m_Constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        num = constraintCount = this.m_ConstraintCount;
      else if (this.m_Constraint == GridLayoutGroup.Constraint.FixedRowCount) {
        num = constraintCount = Mathf.CeilToInt((float)((double)this.rectChildren.Count / (double)this.m_ConstraintCount - 1.0 / 1000.0));
      } else {
        num = 1;
        constraintCount = Mathf.CeilToInt(Mathf.Sqrt((float)this.rectChildren.Count));
      }
      this.SetLayoutInputForAxis((float)this.padding.horizontal + (this.cellSize.x + this.spacing.x) * (float)num - this.spacing.x, (float)this.padding.horizontal + (this.cellSize.x + this.spacing.x) * (float)constraintCount - this.spacing.x, -1f, 0);
    }
  }
}