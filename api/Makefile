.DEFAULT_GOAL := help
POETRY_RUN = poetry run
TEST = pytest $(arg)
CODE = app

COVERAGE_REPORT = htmlcov/status.json
COBERTURA_REPORT = cobertura.xml
COVERAGE_REPORT_FOLDER = $(shell dirname $(COVERAGE_REPORT))

.PHONY: help
help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

.PHONY: test
test: ## Runs pytest
	$(POETRY_RUN) $(TEST)

.PHONY: lint
lint: ## Lint code
	$(POETRY_RUN) pylint --jobs 1 --rcfile=pyproject.toml $(CODE)
	$(POETRY_RUN) ruff check $(CODE)
	$(POETRY_RUN) mypy $(CODE)
	$(POETRY_RUN) black --check $(CODE)
	$(POETRY_RUN) pytest --dead-fixtures --dup-fixtures

.PHONY: format
format: ## Formats all files
	$(POETRY_RUN) black $(CODE)
	$(POETRY_RUN) ruff check --fix $(CODE)

.PHONY: check
check: format lint test ## Format and lint code then run tests

.PHONY: install
install: ## Install dependencies
	poetry install --no-interaction --no-ansi

run-api: ## Run app
	ENV=local $(POETRY_RUN) uvicorn app.api.app:create_app --reload --factory

run-environment: ## Run environment
	@export $$(cat envs/common.env | xargs); export $$(cat envs/local.env | xargs); docker compose -f 'docker-compose-local.yml' up -d

stop-environment: ## Stop environment
	@export $$(cat envs/common.env | xargs); export $$(cat envs/local.env | xargs); docker compose -f 'docker-compose-local.yml' down
