# KarpenkoTest

 Описание:

0. Параметры игры считываются загрузчиком (DatabaseLoader.cs, писал не сам) из конфигурационного файла Resources\ResourceLoader\config.json

1. Сетка игрового поля (80х80) создается менеджером игры (GameManager.cs), после чего им же порождаются две стороны юнитов(белые люди и зеленые зомби).

2. Все юниты двигаются исключительно по узлам сетки (Используя поиск пути 2D https://github.com/juhgiyo/EpPathFinding.cs). Имеют на себе основной менеджер Unit.cs, а также контролер перемещений UnitMovement.cs, контроллер здоровья UnitHP.cs и контроллер атаки UnitAttack.cs. Юнит движется к ближайшей цели, обновляя ее каждый шаг. 

3. Каждая сторона имеет юнита ближнего боя (куб, нечетные при порождении) и юнита дальнего боя (капсуль, четные). Юнит дальнего боя атакует цель с максимальной дистанции, при невозможности стрельбы ввиду стены, продолжает движение к цели еще на шаг.

4. Пуля юнита дальнего боя - желтый шар.

5. Для прицеливания стрелка все же пришлось использовать коллайдеры, для расчета повреждений используется дистанция - чем дальше, тем слабее удар/выстрел.

6. После смерти юнита, он сообщает об этом при помощи !статического! события. При смерти всех юнитов одной стороны вызывается ф-ия GameManager-а CheckForGameOver(), которая и выдает результат.
