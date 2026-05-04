---
name: unity-game-dev
description: >
  Помогает разрабатывать игры на Unity с использованием C#. Используй этот скилл всегда,
  когда пользователь упоминает Unity, C# скрипты для игр, MonoBehaviour, GameObject, префабы,
  физику Rigidbody, Unity UI, ScriptableObject, анимации Animator, NavMesh, шейдеры HLSL/ShaderGraph,
  оптимизацию (батчинг, LOD, профайлер), сборку билдов, Input System, Cinemachine, Timeline,
  пакеты UPM, а также просит написать игровую логику, архитектуру игры, паттерны для Unity
  (Singleton, Event System, Object Pool, State Machine), или спрашивает как "сделать в Unity".
  Также триггерится на слова: "игровая механика", "персонаж", "камера", "уровень", "спавн",
  "коллизии", "raycast", "корутина", "сцена", "ассет".
---

# Unity Game Development Skill

## Структура ответов

При написании C# кода для Unity **всегда**:
- Указывай `using UnityEngine;` и другие нужные неймспейсы
- Используй `[SerializeField]` вместо `public` для инспектора где возможно
- Добавляй XML-документацию (`/// <summary>`) для публичных методов
- Указывай на потенциальные проблемы с производительностью

---

## 1. Архитектура и паттерны

### Singleton (менеджеры)
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

### Event System (развязка зависимостей)
```csharp
// Через C# Action — предпочтительно для простых случаев
public static event Action<int> OnScoreChanged;
OnScoreChanged?.Invoke(newScore);

// Через UnityEvent — для настройки в инспекторе
[SerializeField] private UnityEvent onPlayerDied;
```

### Object Pool
```csharp
// Unity 2021+ встроенный пул
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

### State Machine (для AI/персонажа)
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

## 2. Физика и движение

### Движение персонажа (Rigidbody)
```csharp
// Используй MovePosition для кинематической физики
// Используй AddForce для динамической
[SerializeField] private float speed = 5f;
private Rigidbody _rb;

void FixedUpdate() {
    Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    _rb.MovePosition(_rb.position + move * speed * Time.fixedDeltaTime);
}
```

### CharacterController
```csharp
private CharacterController _cc;
private Vector3 _velocity;
[SerializeField] private float gravity = -9.81f;

void Update() {
    if (_cc.isGrounded && _velocity.y < 0) _velocity.y = -2f;
    Vector3 move = transform.right * h + transform.forward * v;
    _cc.Move(move * speed * Time.deltaTime);
    _velocity.y += gravity * Time.deltaTime;
    _cc.Move(_velocity * Time.deltaTime);
}
```

### Raycast
```csharp
if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 10f))
{
    Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
    if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
        damageable.TakeDamage(damage);
}
```

---

## 3. Input System (новый)

```csharp
// Установи пакет: com.unity.inputsystem
// Создай Input Actions asset → Generate C# Class

private PlayerControls _controls; // сгенерированный класс

void Awake() {
    _controls = new PlayerControls();
    _controls.Player.Jump.performed += ctx => Jump();
}

void OnEnable() => _controls.Enable();
void OnDisable() => _controls.Disable();
```

---

## 4. ScriptableObject (данные и конфиги)

```csharp
[CreateAssetMenu(menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float fireRate;
    public Sprite icon;
}
```
**Используй для**: характеристик оружия/врагов, настроек уровней, диалогов, балансировки — всего, что не должно быть в сцене.

---

## 5. Корутины и async/await

```csharp
// Корутина — для простых задержек
IEnumerator SpawnWithDelay(float delay) {
    yield return new WaitForSeconds(delay);
    Spawn();
    yield return new WaitUntil(() => condition);
}

// Async/Await (Unity 2023+ или с UniTask)
// Установи UniTask: https://github.com/Cysharp/UniTask
async UniTaskVoid LoadSceneAsync(string sceneName) {
    await SceneManager.LoadSceneAsync(sceneName).ToUniTask(Progress.Create<float>(p => loadingBar.value = p));
}
```

---

## 6. UI (UI Toolkit / uGUI)

### uGUI (классический)
```csharp
[SerializeField] private TextMeshProUGUI scoreText;
[SerializeField] private Button playButton;
[SerializeField] private Slider healthBar;

void Start() {
    playButton.onClick.AddListener(OnPlayClicked);
}
public void UpdateScore(int score) => scoreText.text = $"Score: {score}";
```

### UI Toolkit (современный, Unity 2021+)
```csharp
// Привязка через UXML + USS
var root = GetComponent<UIDocument>().rootVisualElement;
var button = root.Q<Button>("play-button");
button.clicked += OnPlayClicked;
var label = root.Q<Label>("score-label");
label.text = "Score: 0";
```

---

## 7. Анимации

```csharp
private Animator _animator;
private static readonly int SpeedHash = Animator.StringToHash("Speed");
private static readonly int JumpHash = Animator.StringToHash("Jump");

// StringToHash — кэшируй хэши один раз, не передавай строки каждый кадр
void Update() {
    _animator.SetFloat(SpeedHash, _rb.velocity.magnitude);
    if (isJumping) _animator.SetTrigger(JumpHash);
}
```

---

## 8. Оптимизация

| Проблема | Решение |
|---|---|
| Много `FindObjectOfType` | Кэшируй в `Awake/Start`, используй ссылки через инспектор |
| `GetComponent` в `Update` | Кэшируй в `Awake` |
| Много `Instantiate/Destroy` | Object Pool |
| Строки в `Animator.SetBool("name")` | `Animator.StringToHash` |
| Слишком много draw calls | Static/Dynamic Batching, GPU Instancing |
| Тяжёлая физика | Layer Collision Matrix, упрощённые коллайдеры |
| GC Allocations | Избегай `new` в `Update`, используй `struct` вместо `class` для данных |

### Профайлер
- **Window → Analysis → Profiler** — CPU/GPU/Memory
- **Window → Analysis → Frame Debugger** — draw calls
- **Memory Profiler** (пакет) — утечки памяти

---

## 9. Сохранение данных

```csharp
// Простые данные — PlayerPrefs
PlayerPrefs.SetInt("HighScore", score);
int hs = PlayerPrefs.GetInt("HighScore", 0);

// Сложные данные — JSON + файл
[System.Serializable]
public class SaveData { public int level; public float[] position; }

string path = Path.Combine(Application.persistentDataPath, "save.json");
File.WriteAllText(path, JsonUtility.ToJson(data));
SaveData loaded = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
```

---

## 10. Структура проекта (рекомендуемая)

```
Assets/
├── _Project/           ← всё своё сюда
│   ├── Scripts/
│   │   ├── Core/       (GameManager, SceneLoader)
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── UI/
│   │   └── Data/       (ScriptableObjects)
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Art/
│   │   ├── Models/
│   │   ├── Textures/
│   │   └── Animations/
│   ├── Audio/
│   └── UI/
│       ├── UXML/
│       └── USS/
├── Plugins/            ← сторонние ассеты
└── StreamingAssets/    ← файлы, доступные на диске
```

---

## 11. Полезные пакеты (UPM)

| Пакет | Назначение |
|---|---|
| `com.unity.inputsystem` | Новая система ввода |
| `com.unity.cinemachine` | Умные камеры |
| `com.unity.ai.navigation` | NavMesh агенты |
| `com.unity.render-pipelines.universal` | URP (мобайл/инди) |
| `com.unity.render-pipelines.high-definition` | HDRP (AAA) |
| `com.unity.addressables` | Асинхронная загрузка ассетов |
| `com.unity.textmeshpro` | Текст (встроен с 2019+) |
| UniTask (GitHub) | async/await без аллокаций |
| DOTween (Asset Store) | Твининг анимации |

---

## Частые ошибки и решения

- **NullReferenceException в Start**: компонент не назначен в инспекторе → используй `[SerializeField]` и проверяй `if (component == null)`
- **Физика не работает**: объект не имеет Rigidbody или `isKinematic = true`
- **Анимация не переключается**: неверное имя параметра, забыли `SetTrigger` → используй хэши
- **Игра тормозит**: открой Profiler, ищи пики в CPU (обычно виновники — physics, rendering, GC.Collect)
- **Prefab не спавнится**: путь в Resources неверный или prefab не в папке Resources при использовании `Resources.Load`
