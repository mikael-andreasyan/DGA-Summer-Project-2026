using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class ComboComb : MonoBehaviour
{
    [Header("Combo Comb Settings")]
    [Tooltip("The amount of time that the combo is preserved for")]
    [SerializeField] private float comboPreserveTime = 2f; // The amount of time that the combo is preserved for
    private bool isComboPreserved = false; // Whether the combo is preserved or not

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(enableComboPreservation());
        }
    }

    IEnumerator enableComboPreservation()
    {
        isComboPreserved = true;
        yield return new WaitForSeconds(comboPreserveTime);
        isComboPreserved = false;
        Destroy(gameObject); // Destroy the combo comb after the combo preservation time is over
    }

    void Update()
    {
        if (isComboPreserved)
        {
            GameManager.Instance.PreserveCombo();
            transform.position = GameManager.Instance.GetPlayer().position;
        }
    }


}
