# Unity Game Development

## Структура ответов

При написании C# кода для Unity **всегда**:
- Указывай `using UnityEngine;` и другие нужные неймспейсы
- Используй `[SerializeField]` вместо `public` для инспектора где возможно
- Добавляй XML-документацию (`/// <summary>`) для публичных методов
- Указывай на потенциальные проблемы с производительностью

---

## Архитектура и паттерны

### Singleton
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

### Event System
```csharp
public static event Action<int> OnScoreChanged;
OnScoreChanged?.Invoke(newScore);

[SerializeField] private UnityEvent onPlayerDied;
```

### Object Pool
```csharp
private ObjectPool<GameObject> _pool;

void Start() {
    _pool = new ObjectPool<GameObject>(
        createFunc: () => Instantiate(prefab),
        actionOnGet: obj => obj.SetActive(true),
        actionOnRelease: obj => obj.SetActive(false),
        defaultCapacity: 20
    );
}
```

### State Machine
```csharp
public interface IState { void Enter(); void Tick(); void Exit(); }

public class StateMachine
{
    private IState _current;
    public void ChangeState(IState newState) {
        _current?.Exit();
        _current = newState;
        _current.Enter();
    }
    public void Tick() => _current?.Tick();
}
```

---

## Оптимизация

| Проблема | Решение |
|---|---|
| Много `FindObjectOfType` | Кэшируй в `Awake/Start`, используй ссылки через инспектор |
| `GetComponent` в `Update` | Кэшируй в `Awake` |
| Много `Instantiate/Destroy` | Object Pool |
| Строки в `Animator.SetBool("name")` | `Animator.StringToHash` |
| Слишком много draw calls | Static/Dynamic Batching, GPU Instancing |
| GC Allocations | Избегай `new` в `Update`, используй `struct` вместо `class` для данных |

---

## Частые ошибки

- **NullReferenceException в Start**: компонент не назначен в инспекторе → используй `[SerializeField]`
- **Физика не работает**: объект не имеет Rigidbody или `isKinematic = true`
- **Анимация не переключается**: неверное имя параметра → используй хэши
- **Игра тормозит**: открой Profiler, ищи пики в CPU

---

# VS Code + Claude

## Как писать хорошие промпты

| Плохо | Хорошо |
|---|---|
| "Почини это" | "В методе `SpawnEnemy` при `count > 10` выбрасывается NullReferenceException. Вот стектрейс: [...]" |
| "Добавь фичу" | "Добавь в класс `PlayerInventory` метод `SwapItems(int slotA, int slotB)`, используя существующий список `_items`" |
| "Напиши тесты" | "Напиши NUnit тесты для `HealthComponent.TakeDamage()`: проверь что HP не уходит ниже 0, что событие `OnDied` срабатывает при смерти" |

### Шаблон промпта
```
[ЧТО НУЖНО СДЕЛАТЬ]
[КОНТЕКСТ: какой класс/файл/метод]
[ОГРАНИЧЕНИЯ: паттерны, стиль, зависимости]
[ЧТО УЖЕ ПРОБОВАЛ / В ЧЁМ ПРОБЛЕМА]
```

## Code Review — что проверять
- Утечки памяти и подписки на события без отписки
- Потенциальные NullReferenceException
- Нарушения принципа единственной ответственности
- Возможные проблемы с производительностью в Update()

## Советы для Unity-проектов
- Не отправляй `.meta` файлы в промпт — они шум
- Указывай версию Unity — поведение API отличается
- Итерируй: "это хорошо, но теперь добавь обработку случая когда список пустой"
- Проси альтернативы: "покажи 2 разных способа реализовать это"
