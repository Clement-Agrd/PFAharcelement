using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatTreeUI : MonoBehaviour
{
    [Header("Références")]
    public StatTreeData         treeData;
    public Transform            container;
    public GameObject           nodePrefab;

    [Header("XP")]
    public TextMeshProUGUI      xpText;
    public Button               resetButton;

    private List<StatNodeUI>    nodeUIs = new List<StatNodeUI>();

    void OnEnable()
    {
        Build();
        Refresh();
    }

    void Build()
    {
        // Nettoie les anciens éléments
        foreach (Transform child in container)
            Destroy(child.gameObject);
        nodeUIs.Clear();

        foreach (StatNodeData node in treeData.nodes)
        {
            GameObject go  = Instantiate(nodePrefab, container);
            StatNodeUI  ui = go.GetComponent<StatNodeUI>();
            ui.Setup(node, this);
            nodeUIs.Add(ui);
        }

        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(OnReset);
    }

    public void Refresh()
    {
        int availableXP = StatTreeManager.Instance.GetAvailableXP();
        int totalXP     = StatTreeManager.Instance.GetTotalXP();
        xpText.text     = $"XP disponible : {availableXP}  |  Total : {totalXP}";

        foreach (StatNodeUI ui in nodeUIs)
            ui.Refresh();
    }

    public void OnUpgrade(StatNodeData node)
    {
        if (StatTreeManager.Instance.Upgrade(node))
            Refresh();
    }

    void OnReset()
    {
        StatTreeManager.Instance.ResetAll();
        Refresh();
    }
}