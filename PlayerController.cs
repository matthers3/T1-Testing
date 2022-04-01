using System.Collections.Generic;
using Yarn.Unity;
using Dialogues;
using GameManagement;
using UnityEngine;

namespace Player {
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float interactionRadius = 2.0f;
        [SerializeField] private GameObject interactionIndicator = default;
        private Direction8Movement direction8Movement;
        private PlayerAnimations playerAnimations;
        private List<InteractionSpot> interactionSpots;

        public bool isIdle = false;
        public bool runTimerActive = true;
        private float idleCounter = 0.0f;
        [SerializeField] private float idleTime = 45.0f;

        public bool PlayerInactive = false;

        void Start()
        {
            // interactionIndicator.SetActive(false);
            direction8Movement = GetComponent<Direction8Movement>();
            playerAnimations = GetComponentInChildren<PlayerAnimations>();
            interactionSpots = new List<InteractionSpot>();
            idleCounter = 0;
        }

        void Update()
        {
            checkActive();
            checkMovement();
            checkDialogue();
            showInteractionIndicator();
        }

        private void showInteractionIndicator()
        {
            DialogueRunner dialogueRunner = FindObjectOfType<DialogueRunner>();
            bool dialogueRunning = dialogueRunner.IsDialogueRunning == true;

            bool NPCNearby = CheckForNPC();
            bool interactionSpotAvailable = interactionSpots.Count > 0;
            bool showInteraction = CheckForNPC() || interactionSpotAvailable;
            // interactionIndicator.SetActive(showInteraction && !dialogueRunning);
            
            // Visible.
            ActionIndicator actionIndicator = interactionIndicator
                .GetComponent<ActionIndicator>();
            actionIndicator.SetVisible(showInteraction);

            // Is Bloqued or Active.
            if (showInteraction)
            {
                if (NPCNearby)
                {
                    actionIndicator.SetEnabledState(true);
                }
                else if (interactionSpotAvailable && !interactionSpots[0].Bloqued)
                {
                    actionIndicator.SetEnabledState(true);
                }
                else
                {
                    actionIndicator.SetEnabledState(false);
                }
            }

        }

        private void checkActive()
        {
            DialogueRunner dialogueRunner = FindObjectOfType<DialogueRunner>();
            bool dialogueRunning = dialogueRunner.IsDialogueRunning == true;
            bool gamePaused = GameManager.Instance.Pause.PauseActive;
            PlayerInactive = dialogueRunning || gamePaused;
        }

        private void checkIdle(float horizontalAxis, float verticalAxis)
        {
            if (PlayerInactive) 
            {
                return;
            }

            isIdle = false;
            if (runTimerActive)
            {
                if (horizontalAxis == 0 && verticalAxis == 0)
                {
                    if (idleCounter > idleTime)
                    {
                        isIdle = true;
                    }
                    else
                    {
                        idleCounter += Time.deltaTime;
                    }
                }
                else
                {
                    idleCounter = 0;
                }
            }
        }

        private void checkDialogue()
        {
            if (PlayerInactive)
            {
                return;
            }

            // Detect if we want to start a conversation
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Space)) 
            {
                if (!CheckForNearbyNPC())
                {
                    triggerInteractionSpot();
                }
            }
        }

        private void checkMovement()
        {
            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            float verticalAxis = Input.GetAxisRaw("Vertical");

            if (PlayerInactive)
            {
                horizontalAxis = 0;
                verticalAxis = 0;
            }

            direction8Movement.RegisterInput(horizontalAxis, verticalAxis);
            playerAnimations.DetermineDirectionalAnimation(horizontalAxis, verticalAxis, isIdle);
            checkIdle(horizontalAxis, verticalAxis);
        }

        public bool CheckForNPC()
        {
            var allParticipants = new List<NPC> (FindObjectsOfType<NPC> ());
            var target = allParticipants.Find (delegate (NPC p) {
                return string.IsNullOrEmpty (p.talkToNode) == false && // has a conversation node?
                (p.transform.position - this.transform.position)// is in range?
                .magnitude <= interactionRadius;
            });
            return target != null;
        }

        public bool CheckForNearbyNPC ()
        {
            var allParticipants = new List<NPC> (FindObjectsOfType<NPC> ());
            var target = allParticipants.Find (delegate (NPC p) {
                return string.IsNullOrEmpty (p.talkToNode) == false && // has a conversation node?
                (p.transform.position - this.transform.position)// is in range?
                .magnitude <= interactionRadius;
            });
            if (target != null) {
                // Kick off the dialogue at this node.
                target.MountDialogue();
                FindObjectOfType<DialogueRunner> ().StartDialogue (target.talkToNode);
                return true;
            }
            return false;
        }

        public void AddInteractionSpot(InteractionSpot spot)
        {
            if (!interactionSpots.Contains(spot))
            {
                interactionSpots.Add(spot);
            }
        }

        public void RemoveInteractionSpot(InteractionSpot spot)
        {
            interactionSpots.RemoveAll(toRemove => toRemove == spot);
        }

        public void triggerInteractionSpot()
        {
            if (interactionSpots.Count > 0)
            {
                if (!interactionSpots[0].Bloqued)
                {
                    interactionSpots[0].Trigger();
                }
                else
                {

                }
            }
        }

    }
}