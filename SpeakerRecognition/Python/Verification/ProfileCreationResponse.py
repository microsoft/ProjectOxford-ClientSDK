class ProfileCreationResponse:
    """This class encapsulates the response of the creation of a user profile."""

    _PROFILE_ID = 'verificationProfileId'

    def __init__(self, response):
        """Constructor of the ProfileCreationResponse class.

        Arguments:
        response -- the dictionary of the deserialized python response
        """
        self._profile_id = response.get(self._PROFILE_ID, None)

    def get_profile_id(self):
        """Returns the profile ID of the user"""
        return self._profile_id
