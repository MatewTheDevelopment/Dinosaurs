using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CharacterSelectManagement : NetworkBehaviour
{
    [SerializeField] private Transform characterPreviewParent;

    [SerializeField] private GameObject characterSelectDisplay;

    [SerializeField] private GameObject[] playableCharacter, previewCharacter;

    [SerializeField] private Text characterNameText;

    [SerializeField] private string[] charNameList;

    [SerializeField] private float turnSpeed = 90f;

    private int currentCharacterIndex = 0;

    private List<GameObject> characterInstances = new List<GameObject>();

    public override void OnStartClient()
    {
        if (characterPreviewParent.childCount == 0)
        {
            for (int index = 0; index < previewCharacter.Length; index++)
            {
                GameObject characterInstance =
                    Instantiate(previewCharacter[index], characterPreviewParent);

                characterInstance.SetActive(false);

                characterInstances.Add(characterInstance);
            }
        }

        characterInstances[currentCharacterIndex].SetActive(true);

        characterNameText.text = charNameList[currentCharacterIndex];

        characterSelectDisplay.SetActive(true);
    }

    private void Update()
    {
        characterPreviewParent.RotateAround(
            characterPreviewParent.position,
            characterPreviewParent.up,
            turnSpeed * Time.deltaTime);
    }

    public void Select()
    {
        CmdSelect(currentCharacterIndex);
        characterSelectDisplay.SetActive(false);
    }

    [Command(requiresAuthority = true)]
    public void CmdSelect(int characterIndex, NetworkConnectionToClient sender = null)
    {
        GameObject characterInstance = Instantiate(playableCharacter[characterIndex]);
        NetworkServer.Spawn(characterInstance, sender);
    }

    public void Right()
    {
        characterInstances[currentCharacterIndex].SetActive(false);

        currentCharacterIndex = (currentCharacterIndex + 1) % characterInstances.Count;

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = charNameList[currentCharacterIndex];
    }

    public void Left()
    {
        characterInstances[currentCharacterIndex].SetActive(false);

        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
        {
            currentCharacterIndex += characterInstances.Count;
        }

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = charNameList[currentCharacterIndex];
    }
}
