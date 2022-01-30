﻿using GameJam2022.JekyllHyde.Controller;
using GameJam2022.JekyllHyde.Domain.Interface;
using System;
using UnityEngine;

namespace GameJam2022.JekyllHyde.Controller.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [field: SerializeField] private EnemyVfx EnemySprite { get; set; }

        public event Action OnKillPlayer
        {
            add => _onKillPlayer += value;
            remove => _onKillPlayer -= value;
        }

        private Action _onKillPlayer { get; set; }

        public event Action<bool> OnChasingUpdate
        {
            add => _onChasingUpdate += value;
            remove => _onChasingUpdate -= value;
        }

        private Action<bool> _onChasingUpdate { get; set; }


        private IEnemy Enemy;
        private IPlayer Player;
        private float RoomSize;
        private Transform PlayerPos;

        private float KillDistance = 2f;
        private float Speed = 2f;
        private float RunSpeed = 3f;
        private float AlreadyMoved = 0f;

        public void Init(IEnemy enemy, IPlayer player, Transform playerPos, float roomSize)
        {
            PlayerPos = playerPos;
            Enemy = enemy;
            Player = player;
            RoomSize = roomSize;
        }

        // Calculos de distancia do player
        private void FixedUpdate()
        {
            float distance = PlayerPos.position.x - gameObject.transform.position.x;

            if (distance < 0)
                distance = -distance;

            if (!Player.IsHidden && distance < KillDistance)
            {
                _onKillPlayer?.Invoke();
                return;
            }

            if (Enemy.ChaseUpdate(Player.IsHidden, PlayerPos.position.x, distance))
                _onChasingUpdate?.Invoke(Enemy.Chasing);
        }

        // Movimentação do personagem no Update para ser consistente com a do player
        private void Update()
        {
            float currentSpeed;
            if (Enemy.Chasing)
                currentSpeed = RunSpeed;
            else
                currentSpeed = Speed;

            Vector3 move = new Vector3(Enemy.CurrentDirection, 0);
            Vector3 totalMove = move * (currentSpeed * Time.deltaTime);

            transform.position += totalMove;

            AlreadyMoved += (totalMove.x > 0) ? totalMove.x : -totalMove.x;

            if (AlreadyMoved > RoomSize) Destroy(gameObject);
        }
    }
}