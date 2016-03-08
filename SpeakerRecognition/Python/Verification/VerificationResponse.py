class VerificationResponse:
    """This class encapsulates the verification response."""

    _RESULT = 'result'
    _CONFIDENCE = 'confidence'

    def __init__(self, response):
        """Constructor of the VerificationResponse class.

        Arguments:
        response -- the dictionary of the deserialized python response
        """
        self._result = response.get(self._RESULT, None)
        self._confidence = response.get(self._CONFIDENCE, None)

    def get_result(self):
        """Returns whether the voice clip belongs to the profile (Accept / Reject)"""
        return self._result

    def get_confidence(self):
        """Returns the verification confidence"""
        return self._confidence
