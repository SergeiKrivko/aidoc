from plantuml import PlantUML


def uml_render(plantuml_code: str) -> bytes:
    l = plantuml_code.find("@startuml")
    r = plantuml_code.rfind("@enduml")
    if r == -1 or l == -1:
        raise ValueError("Invalid plantuml_code")
    plantuml_code = plantuml_code[l:r + len("@enduml")]

    plantuml = PlantUML(url="http://www.plantuml.com/plantuml/png/")

    with open("temp.txt", "w", encoding="utf-8") as file:
        file.write(plantuml_code)

    with open("temp.txt", "r", encoding="utf-8") as file:
        data = file.read()

    return plantuml.processes(data)


