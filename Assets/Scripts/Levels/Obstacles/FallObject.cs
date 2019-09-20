﻿using UnityEngine;

public class FallObject : MonoBehaviour
{
    [Header("Время до падения")]
    [SerializeField] private float time = 1.2f;

    // Начальный слой
    private int order;
    // Начальная позиция
    private Vector3 position;

    // Ссылки на компоненты
    private Rigidbody2D rigbody;
    private SpriteRenderer sprite;
    private AudioSource audioSource;

    private void Awake()
    {
        rigbody = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Получаем слой объекта
        order = sprite.sortingOrder;
        // Получаем позицию объекта
        position = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Получаем компонент персонажа у конувшегося объекта
        var character = collision.gameObject.GetComponent<Character>();

        // Если персонаж живой
        if (character)
        {
            // Вызываем падение объекта через указанное время
            Invoke("FallPlatform", time);
            // Проигрываем звук падения
            if (Options.sound) audioSource.Play();
        }
    }

    // Падение объекта
    private void FallPlatform()
    {
        // Активируем динамическую физику объекта
        rigbody.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Пробуем получить водный компонент у каснувшегося объекта
        var water = collision.GetComponent<Water>();

        if (water)
            // Переносим объект вниз слоя
            sprite.sortingOrder = 0;
        else
        {
            // Иначе пробуем получить компонент урона
            var damage = collision.GetComponent<DamageObjects>();

            if (damage)
            {
                // Скрываем спрайт
                sprite.enabled = false;
                // Через несколько секунд восстанавливаем
                Invoke("RestoreObject", 2.5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Пробуем получить водный компонент
        var water = collision.GetComponent<Water>();

        // Если объект покинул воду, восстанавливаем его
        if (water) Invoke("RestoreObject", 1.5f);
    }

    // Восстановление объекта
    private void RestoreObject()
    {
        // Отображаем спрайт
        sprite.enabled = true;
        // Восстанавливаем слой
        sprite.sortingOrder = order;

        // Восстанавливаем физику объекта
        rigbody.bodyType = RigidbodyType2D.Kinematic;
        rigbody.velocity *= 0;


        // Восстанавливаем позицию
        transform.position = position;
    }
}