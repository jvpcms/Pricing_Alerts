BINARY_NAME = pricing-alerts
BUILDER_IMAGE = pricingalerts-builder
BUILDER_CONTAINER = pricingalerts-tmp
DOCKERFILE = PricingAlerts/Dockerfile.build

build-unix:
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

clean:
	rm -f ./$(BINARY_NAME) ./$(BINARY_NAME).exe
	docker rmi -f $(BUILDER_IMAGE)

.PHONY: build-unix build-windows clean
