import json


with open(file="text.txt", mode="w", encoding="utf-8") as file:
    file.write("plantuml\n@startuml\nclass Matrix {\n    + Multiply(a [][]float64, b [][]float64) [][]float64\n    + ScalarMultiply(m [][]float64, scalar float64) [][]float64\n}\n@enduml\n")
