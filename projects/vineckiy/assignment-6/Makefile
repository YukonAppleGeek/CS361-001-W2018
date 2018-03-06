# this is a comment

TARGET=StudyUp-FIS


default: pdf

pdf:
	pdflatex ${TARGET}

clean-dvi:
	rm -f ${TARGET}.aux
	rm -f ${TARGET}.log
	rm -f ${TARGET}.dvi
	rm -f ${TARGET}.out
	rm -f ${TARGET}.toc

clean-pdf: clean-dvi
	rm -f ${TARGET}.pdf


clean: clean-pdf



