BINARY_NAME = pricing-alerts
BUILDER_IMAGE = pricingalerts-builder
BUILDER_CONTAINER = pricingalerts-tmp
DOCKERFILE = PricingAlerts/Dockerfile.build

build-linux:
	docker build --build-arg RUNTIME=linux-x64 -t $(BUILDER_IMAGE) -f $(DOCKERFILE) .
	docker create --name $(BUILDER_CONTAINER) $(BUILDER_IMAGE)
	docker cp $(BUILDER_CONTAINER):/app/out/PricingAlerts ./$(BINARY_NAME)
	docker rm $(BUILDER_CONTAINER)
	chmod +x ./$(BINARY_NAME)

build-windows:
	docker build --build-arg RUNTIME=win-x64 -t $(BUILDER_IMAGE) -f $(DOCKERFILE) .
	docker create --name $(BUILDER_CONTAINER) $(BUILDER_IMAGE)
	docker cp $(BUILDER_CONTAINER):/app/out/PricingAlerts.exe ./$(BINARY_NAME).exe
	docker rm $(BUILDER_CONTAINER)

build-macos-arm:
	docker build --build-arg RUNTIME=osx-arm64 -t $(BUILDER_IMAGE) -f $(DOCKERFILE) .
	docker create --name $(BUILDER_CONTAINER) $(BUILDER_IMAGE)
	docker cp $(BUILDER_CONTAINER):/app/out/PricingAlerts ./$(BINARY_NAME)-macos-arm
	docker rm $(BUILDER_CONTAINER)
	chmod +x ./$(BINARY_NAME)-macos-arm

build-macos-intel:
	docker build --build-arg RUNTIME=osx-x64 -t $(BUILDER_IMAGE) -f $(DOCKERFILE) .
	docker create --name $(BUILDER_CONTAINER) $(BUILDER_IMAGE)
	docker cp $(BUILDER_CONTAINER):/app/out/PricingAlerts ./$(BINARY_NAME)-macos-intel
	docker rm $(BUILDER_CONTAINER)
	chmod +x ./$(BINARY_NAME)-macos-intel

clean:
	rm -f ./$(BINARY_NAME) ./$(BINARY_NAME).exe ./$(BINARY_NAME)-macos-arm ./$(BINARY_NAME)-macos-intel
	docker rmi -f $(BUILDER_IMAGE)

.PHONY: build-linux build-windows build-macos-arm build-macos-intel clean
