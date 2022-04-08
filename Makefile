DOCKER_TAG=hesamian
DOCKER_RUN=docker run -v `pwd`:/root --rm -w /root ${DOCKER_TAG}
TEST_DIR=test_cases

dockerbuild: App/App.csproj
	docker build . -t ${DOCKER_TAG}

%.run: %.txt dockerbuild 
	${DOCKER_RUN} $<

%.out: %.txt dockerbuild 
	touch $@ && ${DOCKER_RUN} $< > $@

%.test: %.txt %.out
	python3 evaluate.py $^

.PHONY: regression
regression: $(patsubst %.txt,%.test,$(wildcard $(TEST_DIR)/*.txt))
	echo "Running regression"

clean:
	rm -f *.out $(TEST_DIR)/*.out