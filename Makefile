DOCKER_TAG=hesamian
DOCKER_RUN=docker run -i -v `pwd`:/root --rm -w /root --log-driver=none -a stdin -a stdout -a stderr ${DOCKER_TAG} 
TEST_DIR=test_cases

dockerbuild: App/App.csproj
	docker build . -t ${DOCKER_TAG}

%.run: %.txt dockerbuild 
	${DOCKER_RUN} $<

%.out: %.txt dockerbuild
	d=$$(date +%s) \
		; bash -c "cat <(${DOCKER_RUN} $<) > $@" \
		&& echo "*** Runtime took $$(($$(date +%s)-d)) seconds ***"

%.test: %.txt %.out
	python3 evaluate.py $^

.PHONY: regression
regression: $(patsubst %.txt,%.test,$(wildcard $(TEST_DIR)/*.txt))
	echo "Running regression"

clean:
	rm -f *.out $(TEST_DIR)/*.out