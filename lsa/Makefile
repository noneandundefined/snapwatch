.PHONY: fmt lint test

fmt:
	gofmt -w .
	goimports -w .

lint:
	golangci-lint run

check: fmt lint test
	@echo "All checks passed!"
build:
	@echo "Running build..."
	@go build -o bin/lsa .

dll:
	@go build -o lsa.dll -buildmode=c-shared .

test:
	@go test -v ./...

run: build
	@./bin/lsa
