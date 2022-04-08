DOCKER_TAG=hesamian
DOCKER_RUN=docker run -v `pwd`:/root -it --rm -w /root ${DOCKER_TAG}
TEST_DIR=test_cases

dockerbuild: App/App.csproj
	docker build . -t ${DOCKER_TAG}

%.run: %.txt dockerbuild 
	${DOCKER_RUN} $<

%.out: %.txt dockerbuild
	${DOCKER_RUN} $< > $@

%.test: %.out
	python3 evaluate.py $<

.PHONY: regression
regression: $(TEST_DIR)/*.txt
	for file in $^ ; do \
    	echo $(basename $${file}).test ; \
    done